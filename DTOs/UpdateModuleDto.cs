﻿namespace EndavaGrowthspace.DTOs
{
    public class UpdateModuleDto
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int? Order { get; set; }
    }
}
