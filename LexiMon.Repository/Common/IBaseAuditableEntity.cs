﻿namespace LexiMon.Repository.Common;

public interface IBaseAuditableEntity : IBaseEntity
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
    DateTimeOffset DeletedAt { get; set; }
}