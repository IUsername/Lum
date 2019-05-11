using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lum.Annotations;

namespace Lum.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private double _cpu;
        private double _cpuTemp;
        private double _gpu;
        private double _gpuTemp;
        private double _memory;

        public DashboardViewModel()
        {
            Start();
        }

        public double CPU
        {
            get => _cpu;
            set
            {
                if (value == _cpu)
                {
                    return;
                }

                _cpu = value;
                OnPropertyChanged();
            }
        }

        public double CPUTemp
        {
            get => _cpuTemp;
            set
            {
                if (value == _cpuTemp)
                {
                    return;
                }

                _cpuTemp = value;
                OnPropertyChanged();
            }
        }

        public double GPU
        {
            get => _gpu;
            set
            {
                if (value == _gpu)
                {
                    return;
                }

                _gpu = value;
                OnPropertyChanged();
            }
        }

        public double GPUTemp
        {
            get => _gpuTemp;
            set
            {
                if (value == _gpuTemp)
                {
                    return;
                }

                _gpuTemp = value;
                OnPropertyChanged();
            }
        }

        public double Memory
        {
            get => _memory;
            set
            {
                if (value == _memory)
                {
                    return;
                }

                _memory = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Start()
        {
            var rand = new Random();
            CPU = rand.NextDouble();
            CPUTemp = rand.NextDouble() * 100;
            GPU = rand.NextDouble();
            GPUTemp = rand.NextDouble() * 100;
            Memory = rand.NextDouble();
            while (true)
            {
                CPU = Clamp(0, 1, CPU + (rand.NextDouble() - 0.5) * 0.03);
                CPUTemp = Clamp(0, 100, CPUTemp + (rand.NextDouble() - 0.5) * 3);
                GPU = Clamp(0, 1, GPU + (rand.NextDouble() - 0.5) * 0.02);
                GPUTemp = Clamp(0, 100, GPUTemp + (rand.NextDouble() - 0.5) * 6);
                Memory = Clamp(0, 1, Memory + (rand.NextDouble() - 0.5) * 0.05);
                await Task.Delay(1200);
            }
        }

        private static double Clamp(double min, double max, double v) => Math.Min(max, Math.Max(min, v));
    }
}