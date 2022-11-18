using AutoMapper;
using Loggers.CSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loggers.TransformLogger
{
    public class TransformLogger<TRecordSource, TRecordTarget> : ILogger<TRecordSource> where TRecordSource : IRecord where TRecordTarget : IRecord
    {
        ILogger<TRecordTarget> logger;
        Profile profile;
        IMapper mapper;

        public TransformLogger(ILogger<TRecordTarget> logger, Profile profile)
        {
            this.logger = logger;
            this.profile = profile;
        }

        public void Init()
        {
            this.mapper = new Mapper(new MapperConfiguration(options =>
            {
                options.AddProfile(this.profile);
            }));
        }

        public async Task Log(TRecordSource param)
        {
            TRecordTarget target = this.mapper.Map<TRecordSource, TRecordTarget>(param);
            await this.logger.Log(target);
        }
    }
}
