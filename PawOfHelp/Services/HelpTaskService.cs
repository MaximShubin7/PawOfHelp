// Services/HelpTaskService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Animal;
using PawOfHelp.DTOs.HelpTask;
using PawOfHelp.DTOs.Public;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class HelpTaskService : IHelpTaskService
{
    private readonly AppDbContext _context;
    private readonly IAvailabilityService _availabilityService;

    public HelpTaskService(AppDbContext context, IAvailabilityService availabilityService)
    {
        _context = context;
        _availabilityService = availabilityService;
    }

    private async Task<string> GetUserRoleAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return "Неизвестно";

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == user.RoleId);

        return role?.Name ?? "Неизвестно";
    }

    private async Task<List<short>> GetCompetencyIdsByNamesAsync(List<string>? competencyNames)
    {
        if (competencyNames == null || !competencyNames.Any())
            return new List<short>();

        var competencyIds = new List<short>();

        foreach (var name in competencyNames.Distinct())
        {
            var competency = await _context.Competencies
                .FirstOrDefaultAsync(c => c.Name == name);

            if (competency == null)
                throw new Exception($"Компетенция '{name}' не найдена");

            competencyIds.Add(competency.Id);
        }

        return competencyIds;
    }

    private async Task<List<short>> GetLocationIdsByNamesAsync(List<string>? locationNames)
    {
        if (locationNames == null || !locationNames.Any())
            return new List<short>();

        var locationIds = new List<short>();

        foreach (var name in locationNames.Distinct())
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.Name == name);

            if (location == null)
                throw new Exception($"Локация '{name}' не найдена");

            locationIds.Add(location.Id);
        }

        return locationIds;
    }

    private async Task<List<short>> GetPreferenceIdsByNamesAsync(List<string>? preferenceNames)
    {
        if (preferenceNames == null || !preferenceNames.Any())
            return new List<short>();

        var preferenceIds = new List<short>();

        foreach (var name in preferenceNames.Distinct())
        {
            var preference = await _context.Preferences
                .FirstOrDefaultAsync(p => p.Name == name);

            if (preference == null)
                throw new Exception($"Предпочтение '{name}' не найдено");

            preferenceIds.Add(preference.Id);
        }

        return preferenceIds;
    }

    public async Task<HelpTaskResponseDto> CreateHelpTaskAsync(Guid creatorId, CreateHelpTaskDto dto)
    {
        var creator = await _context.Users.FirstOrDefaultAsync(u => u.Id == creatorId);
        if (creator == null)
            throw new Exception("Пользователь не найден");

        var userRole = await GetUserRoleAsync(creatorId);

        if (userRole == "Организация" && dto.IsTaskOverexposure)
            throw new Exception("Организации не могут создавать задачи передержки");

        if (userRole == "Волонтёр" && !dto.IsTaskOverexposure)
            throw new Exception("Волонтёры могут создавать только задачи передержки");

        if (dto.StartedAt >= dto.EndedAt)
            throw new Exception("Дата окончания должна быть позже даты начала");

        var competencyIds = await GetCompetencyIdsByNamesAsync(dto.Competencies);
        var locationIds = await GetLocationIdsByNamesAsync(dto.Locations);

        var task = new HelpTask
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Title = dto.Title,
            Description = dto.Description,
            RequiredVolunteers = dto.RequiredVolunteers,
            IsTaskOverexposure = dto.IsTaskOverexposure,
            StartedAt = dto.StartedAt,
            EndedAt = dto.EndedAt,
            CreatedAt = DateTime.UtcNow,
            CountResponses = 0
        };

        _context.HelpTasks.Add(task);
        await _context.SaveChangesAsync();

        if (dto.AnimalIds != null && dto.AnimalIds.Any())
        {
            foreach (var animalId in dto.AnimalIds.Distinct())
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == animalId);
                if (animal != null)
                {
                    _context.TaskAnimals.Add(new TaskAnimal
                    {
                        TaskId = task.Id,
                        AnimalId = animalId
                    });
                }
            }
        }

        if (competencyIds.Any())
        {
            foreach (var competencyId in competencyIds)
            {
                _context.TaskCompetencies.Add(new TaskCompetency
                {
                    TaskId = task.Id,
                    CompetencyId = competencyId
                });
            }
        }

        if (locationIds.Any())
        {
            foreach (var locationId in locationIds)
            {
                _context.TaskLocations.Add(new TaskLocation
                {
                    TaskId = task.Id,
                    LocationId = locationId
                });
            }
        }

        await _context.SaveChangesAsync();

        return await GetHelpTaskByIdAsync(task.Id);
    }

    public async Task<HelpTaskResponseDto> UpdateHelpTaskAsync(Guid taskId, Guid userId, UpdateHelpTaskDto dto)
    {
        var task = await _context.HelpTasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.CreatorId == userId);

        if (task == null)
            throw new Exception("Задача не найдена или доступ запрещён");

        if (dto.Title != null)
            task.Title = dto.Title;

        if (dto.Description != null)
            task.Description = dto.Description;

        if (dto.RequiredVolunteers.HasValue)
            task.RequiredVolunteers = dto.RequiredVolunteers.Value;

        if (dto.StartedAt.HasValue)
            task.StartedAt = dto.StartedAt.Value;

        if (dto.EndedAt.HasValue)
            task.EndedAt = dto.EndedAt.Value;

        if (dto.StartedAt.HasValue && dto.EndedAt.HasValue && dto.StartedAt.Value >= dto.EndedAt.Value)
            throw new Exception("Дата окончания должна быть позже даты начала");

        if (dto.AnimalIds != null)
        {
            var existingAnimals = await _context.TaskAnimals
                .Where(ta => ta.TaskId == taskId)
                .ToListAsync();
            _context.TaskAnimals.RemoveRange(existingAnimals);

            foreach (var animalId in dto.AnimalIds.Distinct())
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == animalId);
                if (animal != null)
                {
                    _context.TaskAnimals.Add(new TaskAnimal
                    {
                        TaskId = taskId,
                        AnimalId = animalId
                    });
                }
            }
        }

        if (dto.Competencies != null)
        {
            var competencyIds = await GetCompetencyIdsByNamesAsync(dto.Competencies);

            var existingCompetencies = await _context.TaskCompetencies
                .Where(tc => tc.TaskId == taskId)
                .ToListAsync();
            _context.TaskCompetencies.RemoveRange(existingCompetencies);

            foreach (var competencyId in competencyIds)
            {
                _context.TaskCompetencies.Add(new TaskCompetency
                {
                    TaskId = taskId,
                    CompetencyId = competencyId
                });
            }
        }

        if (dto.Locations != null)
        {
            var locationIds = await GetLocationIdsByNamesAsync(dto.Locations);

            var existingLocations = await _context.TaskLocations
                .Where(tl => tl.TaskId == taskId)
                .ToListAsync();
            _context.TaskLocations.RemoveRange(existingLocations);

            foreach (var locationId in locationIds)
            {
                _context.TaskLocations.Add(new TaskLocation
                {
                    TaskId = taskId,
                    LocationId = locationId
                });
            }
        }

        await _context.SaveChangesAsync();

        return await GetHelpTaskByIdAsync(task.Id);
    }

    public async Task DeleteHelpTaskAsync(Guid taskId, Guid userId)
    {
        var task = await _context.HelpTasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.CreatorId == userId);

        if (task == null)
            throw new Exception("Задача не найдена или доступ запрещён");

        _context.HelpTasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<HelpTaskResponseDto> GetHelpTaskByIdAsync(Guid taskId)
    {
        var task = await _context.HelpTasks
            .Include(t => t.Creator)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            throw new Exception("Задача не найдена");

        var animals = await _context.TaskAnimals
            .Where(ta => ta.TaskId == taskId)
            .Include(ta => ta.Animal)
            .Select(ta => new AnimalShortResponseDto
            {
                Id = ta.Animal.Id,
                Name = ta.Animal.Name,
                PhotoUrl = ta.Animal.PhotoUrl
            })
            .ToListAsync();

        var competencies = await _context.TaskCompetencies
            .Where(tc => tc.TaskId == taskId)
            .Include(tc => tc.Competency)
            .Select(tc => tc.Competency.Name)
            .ToListAsync();

        var locations = await _context.TaskLocations
            .Where(tl => tl.TaskId == taskId)
            .Include(tl => tl.Location)
            .Select(tl => tl.Location.Name)
            .ToListAsync();

        var workers = await _context.Workers
            .Where(w => w.TaskId == taskId)
            .Include(w => w.User)
            .Select(w => new PublicProfileDto
            {
                Id = w.User.Id,
                Name = w.User.Name
            })
            .ToListAsync();

        return new HelpTaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            RequiredVolunteers = task.RequiredVolunteers,
            CountResponses = task.CountResponses,
            IsTaskOverexposure = task.IsTaskOverexposure,
            StartedAt = task.StartedAt,
            EndedAt = task.EndedAt,
            CreatedAt = task.CreatedAt,
            Creator = new PublicProfileDto
            {
                Id = task.Creator.Id,
                Name = task.Creator.Name
            },
            Animals = animals,
            Competencies = competencies,
            Locations = locations,
            Workers = workers
        };
    }

    public async Task<HelpTaskListResponseDto> GetFeedTasksAsync(Guid userId, HelpTaskFilterDto filter, int offset, int limit)
    {
        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var query = _context.HelpTasks
            .Include(t => t.Creator)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(t => t.Title.Contains(filter.Search) || (t.Description != null && t.Description.Contains(filter.Search)));
        }

        if (filter.Locations != null && filter.Locations.Any())
        {
            query = query.Where(t => _context.TaskLocations.Any(tl => tl.TaskId == t.Id && filter.Locations.Contains(tl.Location.Name)));
        }

        if (filter.Competencies != null && filter.Competencies.Any())
        {
            query = query.Where(t => _context.TaskCompetencies.Any(tc => tc.TaskId == t.Id && filter.Competencies.Contains(tc.Competency.Name)));
        }

        if (filter.IsTaskOverexposure.HasValue)
        {
            query = query.Where(t => t.IsTaskOverexposure == filter.IsTaskOverexposure.Value);
        }

        if (filter.StartedAfter.HasValue)
        {
            query = query.Where(t => t.StartedAt >= filter.StartedAfter.Value);
        }

        if (filter.StartedBefore.HasValue)
        {
            query = query.Where(t => t.StartedAt <= filter.StartedBefore.Value);
        }

        query = query.Where(t => t.EndedAt > DateTime.UtcNow);

        bool hasAnyFilter = filter.Search != null ||
                            (filter.Locations != null && filter.Locations.Any()) ||
                            (filter.Competencies != null && filter.Competencies.Any()) ||
                            filter.IsTaskOverexposure.HasValue ||
                            filter.StartedAfter.HasValue ||
                            filter.StartedBefore.HasValue;

        if (!hasAnyFilter)
        {
            var user = await _context.Users
                .Include(u => u.Location)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null && user.LocationId.HasValue)
            {
                var locationName = await _context.Locations
                    .Where(l => l.Id == user.LocationId.Value)
                    .Select(l => l.Name)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(locationName))
                {
                    query = query.Where(t => _context.TaskLocations.Any(tl => tl.TaskId == t.Id && tl.Location.Name == locationName));
                }
            }

            var userCompetencies = await _context.UserCompetencies
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Competency)
                .Select(uc => uc.Competency.Name)
                .ToListAsync();

            if (userCompetencies.Any())
            {
                query = query.Where(t => _context.TaskCompetencies.Any(tc => tc.TaskId == t.Id && userCompetencies.Contains(tc.Competency.Name)));
            }

            var userPreferences = await _context.UserPreferences
                .Where(up => up.UserId == userId)
                .Include(up => up.Preference)
                .Select(up => up.Preference.Name)
                .ToListAsync();

            if (userPreferences.Any())
            {
                query = query.Where(t => _context.TaskAnimals.Any(ta => ta.TaskId == t.Id && userPreferences.Contains(ta.Animal.AnimalType.Name)));
            }

            var taskIds = await query.Select(t => t.Id).ToListAsync();
            var availableTaskIds = await _availabilityService.FilterTasksByUserAvailabilityAsync(userId, taskIds);
            query = query.Where(t => availableTaskIds.Contains(t.Id));
        }

        var totalCount = await query.CountAsync();

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var resultTasks = new List<HelpTaskResponseDto>();
        foreach (var task in tasks)
        {
            resultTasks.Add(await GetHelpTaskByIdAsync(task.Id));
        }

        return new HelpTaskListResponseDto
        {
            Tasks = resultTasks,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }

    public async Task<HelpTaskListResponseDto> GetTasksByCreatorAsync(Guid creatorId, int offset, int limit)
    {
        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var query = _context.HelpTasks
            .Include(t => t.Creator)
            .Where(t => t.CreatorId == creatorId);

        var totalCount = await query.CountAsync();

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var resultTasks = new List<HelpTaskResponseDto>();
        foreach (var task in tasks)
        {
            resultTasks.Add(await GetHelpTaskByIdAsync(task.Id));
        }

        return new HelpTaskListResponseDto
        {
            Tasks = resultTasks,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }

    public async Task<HelpTaskListResponseDto> GetTasksByWorkerAsync(Guid workerId, int offset, int limit)
    {
        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var taskIds = await _context.Workers
            .Where(w => w.UserId == workerId)
            .Select(w => w.TaskId)
            .ToListAsync();

        var query = _context.HelpTasks
            .Include(t => t.Creator)
            .Where(t => taskIds.Contains(t.Id));

        var totalCount = await query.CountAsync();

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var resultTasks = new List<HelpTaskResponseDto>();
        foreach (var task in tasks)
        {
            resultTasks.Add(await GetHelpTaskByIdAsync(task.Id));
        }

        return new HelpTaskListResponseDto
        {
            Tasks = resultTasks,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }

    public async Task CompleteAndDeleteTaskAsync(Guid taskId, Guid userId)
    {
        var task = await _context.HelpTasks
            .Include(t => t.Creator)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            throw new Exception("Задача не найдена");

        if (task.CreatorId != userId)
            throw new Exception("Только создатель может завершить задачу");

        var workers = await _context.Workers
            .Where(w => w.TaskId == taskId)
            .ToListAsync();

        foreach (var worker in workers)
        {
            var user = await _context.Users.FindAsync(worker.UserId);
            if (user != null)
            {
                user.CountTasks++;
                _context.Users.Update(user);
            }
        }

        var creator = await _context.Users.FindAsync(task.CreatorId);
        if (creator != null)
        {
            creator.CountTasks++;
            _context.Users.Update(creator);
        }

        _context.HelpTasks.Remove(task);

        await _context.SaveChangesAsync();
    }
}