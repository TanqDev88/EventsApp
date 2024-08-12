using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticketera.Entities;
using Ticketera.Events;
using Ticketera.Extensions;
using Ticketera.TicketCategories;
using Ticketera.TicketSectors;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Ticketera.Migrations.SeedData
{
    public class EventsDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IRepository<Event, long> _eventRepository;
        private readonly IRepository<EventDate, long> _eventDateRepository;
        private readonly IRepository<FileAttachment, long> _fileAttachmentRepository;
        private readonly IRepository<TicketCategory, long> _ticketCategoryRepository;
        private readonly IRepository<TicketSector, long> _ticketSectorRepository;
        private readonly IConfiguration _configuration;
        private readonly IIdentityUserRepository _identityUserRepository;

        public EventsDataSeederContributor(
                        ICurrentTenant currentTenant,
                        IRepository<Event, long> eventRepository,
                        IRepository<FileAttachment, long> fileAttachmentRepository,
                        IRepository<TicketCategory, long> ticketCategoryRepository,
                        IRepository<TicketSector, long> ticketSectorRepository,
                        IConfiguration configuration,
                        IRepository<EventDate, long> eventDateRepository,
                        IIdentityUserRepository identityUserRepository)
        {
            _currentTenant = currentTenant;
            _eventRepository = eventRepository;
            _fileAttachmentRepository = fileAttachmentRepository;
            _ticketCategoryRepository = ticketCategoryRepository;
            _ticketSectorRepository = ticketSectorRepository;
            _configuration = configuration;
            _eventDateRepository = eventDateRepository;
            _identityUserRepository = identityUserRepository;
        }


        public async Task SeedAsync(DataSeedContext context)
        {
            #if DEBUG
            await AddDataToEventInitial(context);
            #endif
        }

        private async Task AddDataToEventInitial(DataSeedContext context)
        {
            await ResetData();

            var adminUser = await _identityUserRepository
                .FindByNormalizedUserNameAsync("ADMIN");

            var category1 = new TicketCategory()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Popular",
                Description = "Entrada general"
            };

            var category2 = new TicketCategory()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Platea",
                Description = "Entrada para platea"
            };

            var category3 = new TicketCategory()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Campo",
                Description = "Entrada para campo"
            };

            var category4 = new TicketCategory()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Medio Campo",
                Description = "Entrada para medio campo"
            };

            var category5 = new TicketCategory()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Campo VIP",
                Description = "Entrada para campo VIP"
            };
            var category6 = new TicketCategory()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Campo VIP 2",
                Description = "Entrada para campo VIP 2"
            };

            var listcats = new List<TicketCategory>() { category1, category2, category3, category4, category5, category6 };

            var sector1 = new TicketSector()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Sector A",
                Description = "Sector A"
            };

            var sector2 = new TicketSector()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Sector B",
                Description = "Sector B"
            };

            var sector3 = new TicketSector()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Sector C",
                Description = "Sector C"
            };

            var sector4 = new TicketSector()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Sector D",
                Description = "Sector D"
            };

            var sector5 = new TicketSector()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Sector E",
                Description = "Sector E"
            };
            var sector6 = new TicketSector()
            {
                CreatorId = Guid.NewGuid(),
                Name = "Sector F",
                Description = "Sector F"
            };

            var listSectors = new List<TicketSector>() { sector1, sector2, sector3, sector4, sector5, sector6 };

            var ev1 = new Event()
            {
                Name = "Fiesta de disfraces",
                Description = "Gran fiesta de disfraces en La Comuna el viernes a la noche",
                IsMain = true,
                Place = "Junin 123, San Luis"
            };
            ev1.Code = ev1.Name.ToLower().Replace(" ","-");

            var ev2 = new Event()
            {
                Name = "Fiesta de la espuma",
                Description = "Gran fiesta de la espuma en Cha Cha Disco Pub viernes a la noche",
                IsMain = true,
                Place = "Rivadavia 123, Mendoza"
            };
            ev2.Code = ev2.Name.ToLower().Replace(" ", "-");

            var ev3 = new Event()
            {
                Name = "Fiesta de la cerveza",
                Description = "Gran fiesta de la cerveza en el Infierno Disco Pub el sabado a la noche",
                IsMain = true,
                Place = "Lujan 123, San Juan"
            }; 
            ev3.Code = ev3.Name.ToLower().Replace(" ", "-");

            var ev4 = new Event()
            {
                Name = "Teatro para niños",
                Description = "Obra de teatro para niños, relatos y cuentos para niños llevados a la tetralidad",
                IsMain = true,
                Place = "San Martin 123, Cordoba"
            };
            ev4.Code = ev4.Name.ToLower().Replace(" ", "-");

            var ev5 = new Event()
            {
                Name = "DJ Manu EFE en Vivo",
                Description = "Gran fiesta en Aqua Barra con la presencia de DJ Manu EFE, dj reconocido desde Mexico",
                IsMain = true,
                Place = "Genral Paz 123, Santa Fe"
            };
            ev5.Code = ev5.Name.ToLower().Replace(" ", "-");

            var ev6 = new Event()
            {
                Name = "Fiesta Retro",
                Description = "Gran fiesta Retro en Rivadavia, Mendoza",
                IsMain = true,
                Place = "Lapegue 123, Mendoza"
            };
            ev6.Code = ev6.Name.ToLower().Replace(" ", "-");

            var ev7 = new Event()
            {
                Name = "Fiesta de disfraces - copy",
                Description = "Gran fiesta de disfraces en La Comuna el viernes a la noche",
                Place = "Junin 123, San Luis"
            };
            ev7.Code = ev7.Name.ToLower().Replace(" ", "-");

            var ev8 = new Event()
            {
                Name = "Fiesta de la espuma - copy",
                Description = "Gran fiesta de la espuma en Cha Cha Disco Pub viernes a la noche",
                Place = "Rivadavia 123, Mendoza"
            };
            ev8.Code = ev8.Name.ToLower().Replace(" ", "-");

            var ev9 = new Event()
            {
                Name = "Fiesta de la cerveza - copy",
                Description = "Gran fiesta de la cerveza en el Infierno Disco Pub el sabado a la noche",
                Place = "Lujan 123, San Juan"
            };
            ev9.Code = ev9.Name.ToLower().Replace(" ", "-");

            var ev10 = new Event()
            {
                Name = "Teatro para niños -copy",
                Description = "Obra de teatro para niños, relatos y cuentos para niños llevados a la tetralidad",
                Place = "San Martin 123, Cordoba"
            };
            ev10.Code = ev10.Name.ToLower().Replace(" ", "-");

            var ev11 = new Event()
            {
                Name = "DJ Manu EFE en Vivo - copy",
                Description = "Gran fiesta en Aqua Barra con la presencia de DJ Manu EFE, dj reconocido desde Mexico",
                IsMain = true,
                Place = "Genral Paz 123, Santa Fe"
            };
            ev11.Code = ev11.Name.ToLower().Replace(" ", "-");

            var ev12 = new Event()
            {
                Name = "Fiesta Retro - copy",
                Description = "Gran fiesta Retro en Rivadavia, Mendoza",
                IsMain = true,
                Place = "Lapegue 123, Mendoza"
            };
            ev12.Code = ev12.Name.ToLower().Replace(" ", "-");

            var events = new List<Event>() { ev1, ev2, ev3, ev4, ev5, ev6, ev7, ev8, ev9, ev10, ev11, ev12 };

            var file1 = new FileAttachment()
            {
                FileName = "file1.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file1.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/1037/1000/400.jpg?hmac=gRNl1uL1WNtpvwvx6a9yo2jvOgUx4KpbyNaduPQ0QXY",
                IsDefault = true
            };
            var file2 = new FileAttachment()
            {
                FileName = "file2.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file2.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/152/1000/400.jpg?hmac=7pdocooag0Y8ikJL1L9ikI4X3iy2ieJd5BpboHGEkYg",
                IsDefault = true
            };
            var file3 = new FileAttachment()
            {
                FileName = "file3.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file3.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/984/900/500.jpg?hmac=5lXoZkTSzifDN1Z4RBbBcpHj8DZBmyjhNdLsp9vi3kg",
                IsDefault = true
            };
            var file4 = new FileAttachment()
            {
                FileName = "file4.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file4.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/779/1000/400.jpg?hmac=c0ti_mF-Uqrvb9F6LoVGiY1ay749yI8LM8HqhY7X4UU",
                IsDefault = true
            };
            var file5 = new FileAttachment()
            {
                FileName = "file5.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file5.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/912/1000/400.jpg?hmac=NDfYDXa0xLQ0nFCjAH_-daQm-XZSkXPcBgA0r7Whwck",
                IsDefault = true
            };
            var file6 = new FileAttachment()
            {
                FileName = "file6.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file6.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/784/600/600.jpg?hmac=AEHh9O7vKxHw9MExTQqPqCXDMJ3VlLOyf-FbuSK8AUs",
                IsDefault = true
            };
            var file7 = new FileAttachment()
            {
                FileName = "file7.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file7.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/427/600/600.jpg?hmac=GSoAgqEy2ee2mH1VZsEhfAFO0mpM4fWGBBHn6TlIOTw"
            };
            var file8 = new FileAttachment()
            {
                FileName = "file8.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file8.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/216/600/600.jpg?hmac=pOF7MACz0I4xxklyDpzbOzuWNjKjZBQki1JA4ViKciY"
            };
            var file9 = new FileAttachment()
            {
                FileName = "file9.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file9.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/177/600/600.jpg?hmac=NLpNGTOav6MVzQYy8A-nulwg64bHHUVUWBy_LHsQDwQ"
            };
            var file10 = new FileAttachment()
            {
                FileName = "file10.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file10.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/1033/600/600.jpg?hmac=ZO6oEcRC66GUsbXCqldF4SDW4RVCJE5pw-FgCeXEyjU"
            };
            var file11 = new FileAttachment()
            {
                FileName = "file11.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file11.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/177/600/600.jpg?hmac=NLpNGTOav6MVzQYy8A-nulwg64bHHUVUWBy_LHsQDwQ"
            };
            var file12 = new FileAttachment()
            {
                FileName = "file12.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file12.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/427/600/600.jpg?hmac=GSoAgqEy2ee2mH1VZsEhfAFO0mpM4fWGBBHn6TlIOTw"
            };
            var file13 = new FileAttachment()
            {
                FileName = "file13.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file13.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/1037/1000/400.jpg?hmac=gRNl1uL1WNtpvwvx6a9yo2jvOgUx4KpbyNaduPQ0QXY",
                IsDefault = true
            };
            var file14 = new FileAttachment()
            {
                FileName = "file14.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file14.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/152/1000/400.jpg?hmac=7pdocooag0Y8ikJL1L9ikI4X3iy2ieJd5BpboHGEkYg",
                IsDefault = true
            };
            var file15 = new FileAttachment()
            {
                FileName = "file15.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file15.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/984/900/500.jpg?hmac=5lXoZkTSzifDN1Z4RBbBcpHj8DZBmyjhNdLsp9vi3kg",
                IsDefault = true
            };
            var file16 = new FileAttachment()
            {
                FileName = "file16.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file16.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/779/1000/400.jpg?hmac=c0ti_mF-Uqrvb9F6LoVGiY1ay749yI8LM8HqhY7X4UU",
                IsDefault = true
            };
            var file17 = new FileAttachment()
            {
                FileName = "file17.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file18.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/912/1000/400.jpg?hmac=NDfYDXa0xLQ0nFCjAH_-daQm-XZSkXPcBgA0r7Whwck",
                IsDefault = true
            };
            var file18 = new FileAttachment()
            {
                FileName = "file18.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file18.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/784/600/600.jpg?hmac=AEHh9O7vKxHw9MExTQqPqCXDMJ3VlLOyf-FbuSK8AUs",
                IsDefault = true
            };
            var file19 = new FileAttachment()
            {
                FileName = "file19.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file19.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/427/600/600.jpg?hmac=GSoAgqEy2ee2mH1VZsEhfAFO0mpM4fWGBBHn6TlIOTw"
            };
            var file20 = new FileAttachment()
            {
                FileName = "file20.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file20.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/216/600/600.jpg?hmac=pOF7MACz0I4xxklyDpzbOzuWNjKjZBQki1JA4ViKciY"
            };
            var file21 = new FileAttachment()
            {
                FileName = "file21.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file21.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/177/600/600.jpg?hmac=NLpNGTOav6MVzQYy8A-nulwg64bHHUVUWBy_LHsQDwQ"
            };
            var file22 = new FileAttachment()
            {
                FileName = "file22.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file22.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/1033/600/600.jpg?hmac=ZO6oEcRC66GUsbXCqldF4SDW4RVCJE5pw-FgCeXEyjU"
            };
            var file23 = new FileAttachment()
            {
                FileName = "file23.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file23.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/177/600/600.jpg?hmac=NLpNGTOav6MVzQYy8A-nulwg64bHHUVUWBy_LHsQDwQ"
            };
            var file24 = new FileAttachment()
            {
                FileName = "file24.jpg",
                FileNameGuid = $"{Guid.NewGuid()}{"file24.jpg".GetExtension()}",
                Size = 100,
                Path = "https://fastly.picsum.photos/id/427/600/600.jpg?hmac=GSoAgqEy2ee2mH1VZsEhfAFO0mpM4fWGBBHn6TlIOTw"
            };


            var files = new List<FileAttachment>() { file1, file2, file3, file4, file5, file6, file7, file8, file9, file10, file11, file12, file13, file14, file15, file16, file17, file18, file19, file20, file21, file22, file23, file24 };


            using (_currentTenant.Change(context?.TenantId))
            {
                // -- Add Categories
                for (int i = 0; i < listcats.Count; i++)
                {
                    if (!await _ticketCategoryRepository.AnyAsync(x => x.Name == listcats[i].Name))
                        listcats[i] = await _ticketCategoryRepository.InsertAsync(listcats[i], autoSave: true);
                }

                // -- Add Sectors
                for (int i = 0; i < listSectors.Count; i++)
                {
                    if (!await _ticketSectorRepository.AnyAsync(x => x.Name == listSectors[i].Name))
                        listSectors[i] = await _ticketSectorRepository.InsertAsync(listSectors[i], autoSave: true);
                }

                // -- Add Events
                var o = 12;
                for (var i = 0; i < events.Count; i++)
                {
                    if (await _eventRepository.FirstOrDefaultAsync(x => x.Name == events[i].Name) == null)
                    {
                        // -- Add prices to Event
                        var prices = new List<PriceModel>();
                        var priceInit = 1000;                        
                        for (var e = 0; e < listcats.Count; e++)
                        {
                            prices.Add(new PriceModel
                            {
                                Count = 200,
                                Price = priceInit * e,
                                TicketCategoryId = listcats[e].Id,
                                TicketCategoryName = listcats[e].Name,
                                TicketSectorId = listSectors[e].Id,
                                TicketSectorName = listSectors[e].Name
                            });
                        }

                        events[i].Prices = prices;
                        events[i].SetCreatorIdExternally(adminUser.Id);

                        // -- Insert Event
                        events[i] = await _eventRepository.InsertAsync(events[i], autoSave: true);
                        files[i].EventId = events[i].Id;
                        files[o].EventId = events[i].Id;

                        // -- Insert Files
                        var fileGalery = files[i];
                        var fileDetail = files[o];
                        fileDetail.FileType = Enum.FileAttachmentType.Detail;
                        await _fileAttachmentRepository.InsertAsync(fileGalery, autoSave: true);
                        await _fileAttachmentRepository.InsertAsync(fileDetail, autoSave: true);

                        // -- Insert Event Dates
                        await _eventDateRepository.InsertAsync(new EventDate()
                        {
                            EventId = events[i].Id,
                            StartDate = DateTime.Now.AddDays(i),
                        },true);
                        await _eventDateRepository.InsertAsync(new EventDate()
                        {
                            EventId = events[i].Id,
                            StartDate = DateTime.Now.AddDays(6 + i),
                        },true);
                        o++;
                    }
                }
            }
        }

        private async Task ResetData()
        {
            // -- Delete files
            var files = await _fileAttachmentRepository.GetListAsync();
            var ids = files.Select(e => e.Id).ToList();
            await _fileAttachmentRepository.DeleteAsync(x => ids.Contains(x.Id), autoSave: true);

            // -- Delete Events
            var events = await _eventRepository.GetListAsync();
            ids = events.Select(e => e.Id).ToList();
            await _eventRepository.DeleteAsync(x => ids.Contains(x.Id), autoSave: true);

            // -- Delete Sectors
            var sectors = await _ticketSectorRepository.GetListAsync();
            ids = sectors.Select(e => e.Id).ToList();
            await _ticketSectorRepository.DeleteAsync(x => ids.Contains(x.Id), autoSave: true);

            // -- Delete Sectors
            var categories = await _ticketCategoryRepository.GetListAsync();
            ids = categories.Select(e => e.Id).ToList();
            await _ticketCategoryRepository.DeleteAsync(x => ids.Contains(x.Id), autoSave: true);
        }
    }
}
