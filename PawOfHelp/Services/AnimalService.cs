// Services/AnimalService.cs
using Microsoft.EntityFrameworkCore;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Animal;
using PawOfHelp.Models;
using PawOfHelp.Services.Interfaces;

namespace PawOfHelp.Services;

public class AnimalService : IAnimalService
{
    private readonly AppDbContext _context;
    private readonly IImageKitService _imageKitService;

    public AnimalService(AppDbContext context, IImageKitService imageKitService)
    {
        _context = context;
        _imageKitService = imageKitService;
    }

    public async Task<AnimalResponseDto> CreateAnimalAsync(Guid userId, CreateAnimalDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new Exception("Пользователь не найден");

        var animalType = await _context.AnimalTypes
            .FirstOrDefaultAsync(at => at.Name == dto.AnimalType);

        if (animalType == null)
            throw new Exception($"Тип животного '{dto.AnimalType}' не найден");

        var animal = new Animal
        {
            Id = Guid.NewGuid(),
            AnimalTypeId = animalType.Id,
            Breed = dto.Breed,
            Name = dto.Name,
            Age = dto.Age,
            Health = dto.Health,
            Character = dto.Character,
            SpecialNeeds = dto.SpecialNeeds
        };

        if (dto.Photo != null)
        {
            var photoUrl = await _imageKitService.UploadImageAsync(dto.Photo, "animals", animal.Id.ToString());
            animal.PhotoUrl = photoUrl;
        }
        else if (!string.IsNullOrEmpty(dto.PhotoUrlFromWeb))
        {
            var photoUrl = await _imageKitService.UploadImageFromUrlAsync(dto.PhotoUrlFromWeb, "animals", animal.Id.ToString());
            animal.PhotoUrl = photoUrl;
        }

        _context.Animals.Add(animal);

        var userAnimal = new UserAnimal
        {
            UserId = userId,
            AnimalId = animal.Id
        };
        _context.UserAnimals.Add(userAnimal);

        await _context.SaveChangesAsync();

        return await GetAnimalByIdAsync(animal.Id);
    }

    public async Task<AnimalResponseDto> UpdateAnimalAsync(Guid animalId, Guid userId, UpdateAnimalDto dto)
    {
        var userAnimal = await _context.UserAnimals
            .FirstOrDefaultAsync(ua => ua.AnimalId == animalId && ua.UserId == userId);

        if (userAnimal == null)
            throw new Exception("Животное не найдено или доступ запрещён");

        var animal = await _context.Animals
            .Include(a => a.AnimalType)
            .FirstOrDefaultAsync(a => a.Id == animalId);

        if (animal == null)
            throw new Exception("Животное не найдено");

        if (!string.IsNullOrEmpty(dto.AnimalType))
        {
            var animalType = await _context.AnimalTypes
                .FirstOrDefaultAsync(at => at.Name == dto.AnimalType);

            if (animalType == null)
                throw new Exception($"Тип животного '{dto.AnimalType}' не найден");

            animal.AnimalTypeId = animalType.Id;
        }

        if (dto.Breed != null)
            animal.Breed = dto.Breed;

        if (dto.Name != null)
            animal.Name = dto.Name;

        if (dto.Age.HasValue)
            animal.Age = dto.Age;

        if (dto.Health != null)
            animal.Health = dto.Health;

        if (dto.Character != null)
            animal.Character = dto.Character;

        if (dto.SpecialNeeds != null)
            animal.SpecialNeeds = dto.SpecialNeeds;

        if (dto.Photo != null)
        {
            var photoUrl = await _imageKitService.UploadImageAsync(dto.Photo, "animals", animal.Id.ToString());
            if (photoUrl != null)
                animal.PhotoUrl = photoUrl;
        }
        else if (!string.IsNullOrEmpty(dto.PhotoUrlFromWeb))
        {
            var photoUrl = await _imageKitService.UploadImageFromUrlAsync(dto.PhotoUrlFromWeb, "animals", animal.Id.ToString());
            if (photoUrl != null)
                animal.PhotoUrl = photoUrl;
        }

        await _context.SaveChangesAsync();

        return await GetAnimalByIdAsync(animal.Id);
    }

    public async Task DeleteAnimalAsync(Guid animalId, Guid userId)
    {
        var userAnimal = await _context.UserAnimals
            .FirstOrDefaultAsync(ua => ua.AnimalId == animalId && ua.UserId == userId);

        if (userAnimal == null)
            throw new Exception("Животное не найдено или доступ запрещён");

        _context.UserAnimals.Remove(userAnimal);

        var otherOwners = await _context.UserAnimals.AnyAsync(ua => ua.AnimalId == animalId);
        if (!otherOwners)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == animalId);
            if (animal != null)
                _context.Animals.Remove(animal);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<AnimalResponseDto> GetAnimalByIdAsync(Guid animalId)
    {
        var animal = await _context.Animals
            .Include(a => a.AnimalType)
            .FirstOrDefaultAsync(a => a.Id == animalId);

        if (animal == null)
            throw new Exception("Животное не найдено");

        var owners = await _context.UserAnimals
            .Where(ua => ua.AnimalId == animalId)
            .Include(ua => ua.User)
            .Select(ua => new AnimalOwnerShortDto
            {
                Id = ua.User.Id,
                Name = ua.User.Name
            })
            .ToListAsync();

        return new AnimalResponseDto
        {
            Id = animal.Id,
            AnimalType = animal.AnimalType?.Name ?? string.Empty,
            Breed = animal.Breed,
            Name = animal.Name,
            Age = animal.Age,
            Health = animal.Health,
            Character = animal.Character,
            SpecialNeeds = animal.SpecialNeeds,
            PhotoUrl = animal.PhotoUrl,
            Owners = owners
        };
    }

    public async Task<AnimalListResponseDto> GetOrganizationAnimalsAsync(Guid organizationId, int offset, int limit)
    {
        var organizationExists = await _context.OrganizationDetails.AnyAsync(o => o.UserId == organizationId);
        if (!organizationExists)
            throw new Exception("Организация не найдена");

        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var query = _context.UserAnimals
            .Where(ua => ua.UserId == organizationId)
            .Include(ua => ua.Animal)
            .Select(ua => new AnimalShortResponseDto
            {
                Id = ua.Animal.Id,
                Name = ua.Animal.Name,
                PhotoUrl = ua.Animal.PhotoUrl
            });

        var totalCount = await query.CountAsync();

        var animals = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        return new AnimalListResponseDto
        {
            Animals = animals,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }

    public async Task<AnimalListResponseDto> GetVolunteerAnimalsAsync(Guid volunteerId, int offset, int limit)
    {
        var volunteerExists = await _context.Users.AnyAsync(u => u.Id == volunteerId && u.RoleId == 1);
        if (!volunteerExists)
            throw new Exception("Волонтёр не найден");

        if (limit <= 0 || limit > 50)
            limit = 10;

        if (offset < 0)
            offset = 0;

        var query = _context.UserAnimals
            .Where(ua => ua.UserId == volunteerId)
            .Include(ua => ua.Animal)
            .Select(ua => new AnimalShortResponseDto
            {
                Id = ua.Animal.Id,
                Name = ua.Animal.Name,
                PhotoUrl = ua.Animal.PhotoUrl
            });

        var totalCount = await query.CountAsync();

        var animals = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        return new AnimalListResponseDto
        {
            Animals = animals,
            Offset = offset,
            Limit = limit,
            HasMore = offset + limit < totalCount
        };
    }

    public async Task<List<AnimalShortResponseDto>> GetLatestUserAnimalsAsync(Guid userId)
    {
        var animals = await _context.UserAnimals
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Animal)
            .Take(3)
            .Select(ua => new AnimalShortResponseDto
            {
                Id = ua.Animal.Id,
                Name = ua.Animal.Name,
                PhotoUrl = ua.Animal.PhotoUrl
            })
            .ToListAsync();

        return animals;
    }
}