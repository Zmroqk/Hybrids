using AutoMapper;
using Meta.DataTTP.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.DataTTP
{
    public class MapperProfile : Profile
    {
        public static MapperProfile Profile = new MapperProfile();
        public static Mapper Mapper = new Mapper(new MapperConfiguration(options =>
        {
            options.AddProfile<MapperProfile>();
        }));

        public MapperProfile() { 
            this.CreateMap<TabuRecord, WavyHybridRecord>()
                .ForMember(x => x.CurrentScore
                    , x => x.MapFrom(xx => xx.CurrentSpecimenScore)
                )
                .ForMember(x => x.BestScore
                    , x => x.MapFrom(xx => xx.BestSpecimenScore)
                )
                .ForMember(x => x.Iteration
                    , x => x.MapFrom(xx => xx.Generation)
                );
            this.CreateMap<SimulatedAnnealingRecord, WavyHybridRecord>();
            this.CreateMap<SpecimenWithAge, Specimen>()
                .ForMember(x => x.SpecimenInitialization
                    , x => x.Ignore()
                );
        }
    }
}
