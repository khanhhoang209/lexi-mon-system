using System.ComponentModel.DataAnnotations;
using LexiMon.Repository.Domains;
using LexiMon.Repository.Interfaces;
using LexiMon.Service.ApiResponse;
using LexiMon.Service.Interfaces;
using LexiMon.Service.Models.Requests;
using LexiMon.Service.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class QuestionService : IQuestionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QuestionService> _logger;
    
    public QuestionService(IUnitOfWork unitOfWork, ILogger<QuestionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<ResponseData<List<Guid>>> CreateQuestionAnswerAsync(
        List<QuestionCreateRequestDto> requests,
        CancellationToken ct = default)
    {
        try
        {
            if (requests is null || requests.Count == 0)
            {
                _logger.LogWarning("Emty request!!");
                return new ResponseData<List<Guid>> { Succeeded = false, Message = "Empty request." };
                            
            }
         
            var questionRepo = _unitOfWork.GetRepository<Question, Guid>(); 
            var lessonRepo = _unitOfWork.GetRepository<Lesson, Guid>();
            var customLessonRepo = _unitOfWork.GetRepository<CustomLesson, Guid>();

            var now = DateTimeOffset.UtcNow;
            var toAdd = new List<Question>(); 
            foreach (var r in requests)
            { 
                var lessonId       = ParseGuidOrNull(r.LessonId); 
                var customLessonId = ParseGuidOrNull(r.CustomLessonId);

            // validate owner (y hệt Create hiện tại)
            if (lessonId is not null)
            {
                if (await lessonRepo.GetByIdAsync(lessonId.Value, ct) is null)
                {
                    _logger.LogWarning($"Lesson with id {lessonId} not found!");
                    return new  ResponseData<List<Guid>> { Succeeded = false, Message = $"Lesson with id {lessonId} not found!" };
                                
                }
            }
            else if (customLessonId is not null)
            {
                if (await customLessonRepo.GetByIdAsync(customLessonId.Value, ct) is null)
                {
                    _logger.LogWarning( $"Custom lesson with id {customLessonId} not found!");
                    return new ResponseData<List<Guid>> { Succeeded = false, Message = $"Custom lesson with id {customLessonId} not found!" };
                }
            }
            else
            {
                _logger.LogWarning( $"Either LessonId or CustomLessonId must be provided!");
                return new ResponseData<List<Guid>> {
                    Succeeded = false,
                    Message = "Either LessonId or CustomLessonId must be provided!"
                };
            }

            var questionId = Guid.NewGuid();
            var question = new Question
            { 
                Id = questionId,
                Content = r.Content.Trim(),
                LessonId = lessonId,
                CustomLessonId = customLessonId,
                IsStudied = false,
                CreatedAt = now,
                Answers = r.Answers.Select(a => new Answer
                {
                    Id = Guid.NewGuid(),
                    QuestionId = questionId,
                    Content = a.Content.Trim(),
                    IsCorrect = a.IsCorrect,
                    CreatedAt = now, 
                    Status = true
                }).ToList()
            }; 
            toAdd.Add(question);
            }

            await questionRepo.AddRangeAsync(toAdd, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Questions created successfully");
            return new ResponseData<List<Guid>>
            { 
                Succeeded = true, 
                Message = "Questions created successfully!",
                Data = toAdd.Select(x => x.Id).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to create question and answers: {ex.Message}");
            return new ResponseData<List<Guid>>()
            {
                Succeeded = false,
                Message = $"Failed to create question and answers: {ex.Message}"
            };
        }
    }

   
    public async Task<ServiceResponse> UpdateQuestionReplaceAsync(
        Guid id, QuestionUpdateRequestDto req, CancellationToken ct = default)
    {
        var qRepo = _unitOfWork.GetRepository<Question, Guid>();
        var aRepo = _unitOfWork.GetRepository<Answer, Guid>();
        
        var question = await qRepo.GetByIdAsync(id, ct);
        if (question is null)
        {
            _logger.LogWarning($"Question with id {id} not found!");
            return new ServiceResponse { Succeeded = false, Message = "Question not found!" };
        }
            

        var now = DateTimeOffset.UtcNow;
        question.Content   = req.Content.Trim();
        question.IsStudied = req.IsStudied;
        question.UpdatedAt = now;

        // XÓA  mọi Answer thuộc question TRỰC TIẾP TRÊN DB
        await aRepo.Query()                      
            .Where(a => a.QuestionId == id)
            .ExecuteDeleteAsync(ct);

        // THÊM lại answers theo payload
        var toAdd = req.Answers.Select(x => new Answer
        {
            Id         = Guid.NewGuid(),
            QuestionId = id,
            Content    = x.Content.Trim(),
            IsCorrect  = x.IsCorrect,
            Status     = true,        
            CreatedAt  = now
        }).ToList();

        await aRepo.AddRangeAsync(toAdd, ct);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation("Update successfully!");
            return new ServiceResponse { Succeeded = true, Message = "Question and answers updated successfully!" };
        }
        catch
        {
            // Phòng khi chính bản thân Question vừa bị xóa/sửa ở nơi khác
            return new ServiceResponse { Succeeded = false, Message = "Conflict: question was modified/deleted. Reload and try again." };
        }
    }

    public async Task<ServiceResponse> DeleteQuestionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var questionRepo = _unitOfWork.GetRepository<Question, Guid>();
        var question = await questionRepo.GetByIdAsync(id, cancellationToken);
        if (question is null)
        {
            _logger.LogWarning($"Question with id {id} not found!");
            return new ServiceResponse
            { 
                Succeeded = false, 
                Message = "Question not found!"
            };
        }
          
        
        await questionRepo.RemoveAsync(question, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Questions deleted successfully!");
        return new ServiceResponse
        {
            Succeeded = true,
            Message = "Question deleted successfully!"
        };
    }

    public async Task<ResponseData<QuestionResponseDto>> GetQuestionByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var questionRepo = _unitOfWork.GetRepository<Question, Guid>();

        var dto = await questionRepo.Query(asNoTracking: true)
            .Include(q => q.Answers.Where(a => a.Status))
            .Select(q => new QuestionResponseDto
            {
                QuestionId = q.Id,
                Content = q.Content,
                LessonId = q.LessonId,
                LessonTitle = q.Lesson != null ? q.Lesson.Title : null,
                CustomLessonId = q.CustomLessonId,
                CustomLessonTitle = q.CustomLesson != null ? q.CustomLesson.Title : null,
                IsActive = q.Status,
                IsStudied = q.IsStudied,
                Answers = q.Answers.Select(a => new AnswerResponseDto
                {
                    AnswerId = a.Id,
                    Content = a.Content,
                    IsCorrect = a.IsCorrect
                }).ToList()
            })
            .FirstOrDefaultAsync(q => q.QuestionId == id, cancellationToken);

        if (dto is null)
        {
            _logger.LogWarning("Question not found");
            return new ResponseData<QuestionResponseDto>()
            {
                Succeeded = false,
                Message = "Question not found!"
            };
        }
          

        _logger.LogInformation("Questions retrieved successfully!");
        return new ResponseData<QuestionResponseDto>()
        {
            Succeeded = true,
            Message = "Question retrieved successfully!",
            Data = dto
        };
    }

    public async Task<PaginatedResponse<QuestionLessonResponseDto>> GetQuestionsByLessonIdAsync(
        Guid lessonId, 
        GetQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var lessonRepo = _unitOfWork.GetRepository<Lesson, Guid>();
        var lesson = await lessonRepo.GetByIdAsync(lessonId, cancellationToken);
        if (lesson == null)
        {
            _logger.LogWarning("Lesson not found");
            return new PaginatedResponse<QuestionLessonResponseDto>()
            {
                Succeeded = false,
                Message = "Lesson not found!"
            };
        }
        
        var questionRepo = _unitOfWork.GetRepository<Question, Guid>();
        var query = questionRepo.Query()
            .Where(q => q.LessonId == lessonId && q.Status == true)
            .Include(q => q.Answers.Where(a => a.Status))
            .AsNoTracking();
        if(!string.IsNullOrEmpty(request.QuestionContent))
            query = query.Where(q => q.Content.Contains(request.QuestionContent));
        
        var totalCount = query.Count();
        
        var questions = await query
            .OrderBy(q => q.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(q => new QuestionLessonResponseDto
            {
                QuestionId = q.Id,
                Content = q.Content,
                LessonId = q.LessonId,
                LessonTitle = q.Lesson != null ? q.Lesson.Title : null,
                IsActive = q.Status,
                IsStudied = q.IsStudied,
                Answers = q.Answers.Select(a => new AnswerResponseDto
                {
                    AnswerId = a.Id,
                    Content = a.Content,
                    IsCorrect = a.IsCorrect
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Questions retrieved successfully!");
        return new PaginatedResponse<QuestionLessonResponseDto>()
        {
            Succeeded = true,
            Message = "Questions retrieved successfully!",
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            Data = questions

        };
    }

    public async Task<PaginatedResponse<QuestionCustomLessonResponseDto>> GetQuestionsByCustomLessonIdAsync(
        Guid customLessonId, 
        GetQuestionRequest request,
        CancellationToken cancellationToken = default)
    {
        var customLessonRepo = _unitOfWork.GetRepository<CustomLesson, Guid>();
        var customLesson = await customLessonRepo.GetByIdAsync(customLessonId, cancellationToken);
        if (customLesson == null)
        {
            _logger.LogWarning("Custom lesson not found");
            return new PaginatedResponse<QuestionCustomLessonResponseDto>()
            {
                Succeeded = false,
                Message = "Custom lesson not found!"
            };
        }
        
        var questionRepo = _unitOfWork.GetRepository<Question, Guid>();
        var query = questionRepo.Query()
            .Where(q => q.CustomLessonId == customLessonId)
            .Include(q => q.Answers.Where(a => a.Status))
            .AsNoTracking();
        
        if(!string.IsNullOrEmpty(request.QuestionContent))
            query = query.Where(q => q.Content.Contains(request.QuestionContent));
        
        var totalCount = query.Count();
        
        var questions = await query
            .OrderBy(q => q.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(q => new QuestionCustomLessonResponseDto()
            {
                QuestionId = q.Id,
                Content = q.Content,
                CustomLessonId = q.CustomLessonId,
                CustomLessonTitle = q.CustomLesson != null ? q.CustomLesson.Title : null,
                IsActive = q.Status,
                IsStudied = q.IsStudied,
                Answers = q.Answers.Select(a => new AnswerResponseDto
                {
                    AnswerId = a.Id,
                    Content = a.Content,
                    IsCorrect = a.IsCorrect
                }).ToList()
            })
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Questions retrieved successfully!");
        return new PaginatedResponse<QuestionCustomLessonResponseDto>()
        {
            Succeeded = true,
            Message = "Questions retrieved successfully!",
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            Data = questions

        };
    }
    
    #region Helper
    private static Guid? ParseGuidOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return Guid.TryParse(value, out var guid) ? guid : null;
    }

    #endregion
}

