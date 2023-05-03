using System.Collections.Concurrent;
using Api.Modules.TaskSchedulerModels.Core.Workers;
using WiserTaskScheduler.Core.Workers;

namespace Api.Modules.TaskSchedulerModels
{
    public class ActiveConfigurationModel
    {
        /// <summary>
        /// Gets or sets the version of the active configuration.
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// Gets or sets the collection of workers for each time ID.
        /// </summary>
        public ConcurrentDictionary<int, ConfigurationsWorker> WorkerPerTimeId { get; set; }
    }
}
