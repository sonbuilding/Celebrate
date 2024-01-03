using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUser.Models
{
    public class ProgressReport
    {
        private int count;
        private readonly Action<Progress<int>> action;
        public int value { get; set; }
        public ProgressReport(int count, Action<Progress<int>> action)
        {
            this.count = count;
            this.action = action;
        }

        public async void UIUpdateProgres(Progress<int> progress)
        {
            action?.Invoke(progress);
            await Task.Run(() => BackgroundUpdateProgress(progress));
        }

        private void BackgroundUpdateProgress(IProgress<int> progress)
        {
            var percentComplte = value * 100 / count;
            progress.Report(percentComplte);
        }
    }
}

