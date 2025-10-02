using LexiMon.Repository.Interfaces;
using LexiMon.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace LexiMon.Service.Implements;

public class AnimationService : IAnimationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AnimationService> _logger;

    public AnimationService(IUnitOfWork unitOfWork, ILogger<AnimationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
}