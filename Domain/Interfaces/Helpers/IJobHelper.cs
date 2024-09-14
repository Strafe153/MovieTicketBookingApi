using Quartz;

namespace Domain.Interfaces.Helpers;

public interface IJobHelper
{
    Task RunOneOffJob<T>(JobDataMap? dataMap) where T : IJob;
}
