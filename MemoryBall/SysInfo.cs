using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace MemoryBall
{
    public class SysInfo : INotifyPropertyChanged
    {
        private const int R = 34;
        private const int Rr = 44;
        private const int Offset = 46;

        private readonly double[] _table =
        {
            0, 0.06279052, 0.125333234, 0.187381315, 0.248689887, 0.309016994,
            0.368124553, 0.425779292, 0.481753674, 0.535826795, 0.587785252,
            0.63742399, 0.684547106, 0.728968627, 0.770513243, 0.809016994,
            0.844327926, 0.87630668, 0.904827052, 0.929776486, 0.951056516,
            0.968583161, 0.982287251, 0.992114701, 0.998026728, 1
        };

        public SysInfo()
            => _innerPoint = _outerPoint = _inner = _outer = new Point(Offset, 2);

        public event PropertyChangedEventHandler PropertyChanged;

        #region 信息更新函数

        private void UpdateMemoryInfo()
        {
            switch (_memLoad)
            {
                case < 25:
                    IsLargeArc = false;
                    _inner.X = Offset + R * _table[_memLoad];
                    _outer.X = Offset + Rr * _table[_memLoad];
                    _inner.Y = Offset - R * _table[25 - _memLoad];
                    _outer.Y = Offset - Rr * _table[25 - _memLoad];
                    InnerPoint = _inner;
                    OuterPoint = _outer;
                    return;
                case < 50:
                    IsLargeArc = false;
                    _inner.X = Offset + R * _table[50 - _memLoad];
                    _outer.X = Offset + Rr * _table[50 - _memLoad];
                    _inner.Y = Offset + R * _table[_memLoad - 25];
                    _outer.Y = Offset + Rr * _table[_memLoad - 25];
                    InnerPoint = _inner;
                    OuterPoint = _outer;
                    return;
                case < 75:
                    IsLargeArc = true;
                    _inner.X = Offset - R * _table[_memLoad - 50];
                    _outer.X = Offset - Rr * _table[_memLoad - 50];
                    _inner.Y = Offset + R * _table[75 - _memLoad];
                    _outer.Y = Offset + Rr * _table[75 - _memLoad];
                    InnerPoint = _inner;
                    OuterPoint = _outer;
                    return;
            }

            IsLargeArc = true;

            if (_memLoad < 100)
            {
                _inner.X = Offset - R * _table[100 - _memLoad];
                _inner.Y = Offset - R * _table[_memLoad - 75];
                _outer.X = Offset - Rr * _table[100 - _memLoad];
                _outer.Y = Offset - Rr * _table[_memLoad - 75];
                InnerPoint = _inner;
                OuterPoint = _outer;
                return;
            }

            _inner.X = Offset - R * 0.008726535;
            _inner.Y = Offset - R * 0.999961923;
            _outer.X = Offset - Rr * 0.008726535;
            _outer.Y = Offset - Rr * 0.999961923;

            InnerPoint = _inner;
            OuterPoint = _outer;
        }

        #endregion

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #region 属性

        private Point _inner;
        private Point _innerPoint;

        public Point InnerPoint
        {
            get => _innerPoint;
            private set
            {
                if (_innerPoint == value) return;
                _innerPoint = value;
                OnPropertyChanged(nameof(InnerPoint));
            }
        }

        private Point _outer;
        private Point _outerPoint;

        public Point OuterPoint
        {
            get => _outerPoint;
            private set
            {
                if (_outerPoint == value) return;
                _outerPoint = value;
                OnPropertyChanged(nameof(OuterPoint));
            }
        }

        private bool _isLargeArc;

        public bool IsLargeArc
        {
            get => _isLargeArc;
            set
            {
                if (_isLargeArc == value) return;
                _isLargeArc = value;
                OnPropertyChanged(nameof(IsLargeArc));
            }
        }

        private int _memLoad;

        public int MemLoad
        {
            get => _memLoad;
            set
            {
                if (_memLoad == value) return;
                _memLoad = value;
                UpdateMemoryInfo();
            }
        }

        private string _cpuLoad;

        public string CpuLoad
        {
            get => _cpuLoad;
            set
            {
                if (_cpuLoad == value) return;
                _cpuLoad = value;
                OnPropertyChanged(nameof(CpuLoad));
            }
        }

        private string _netLoad;

        public string NetLoad
        {
            get => _netLoad;
            set
            {
                if (_netLoad == value) return;
                _netLoad = value;
                OnPropertyChanged(nameof(NetLoad));
            }
        }

        private SolidColorBrush _memColor;

        public SolidColorBrush MemColor
        {
            get => _memColor;
            set
            {
                if (_memColor == value) return;
                _memColor = value;
                OnPropertyChanged(nameof(MemColor));
            }
        }

        private SolidColorBrush _cpuColor;

        public SolidColorBrush CpuColor
        {
            get => _cpuColor;
            set
            {
                if (_cpuColor == value) return;
                _cpuColor = value;
                OnPropertyChanged(nameof(CpuColor));
            }
        }

        private SolidColorBrush _netColor;

        public SolidColorBrush NetColor
        {
            get => _netColor;
            set
            {
                if (_netColor == value) return;
                _netColor = value;
                OnPropertyChanged(nameof(NetColor));
            }
        }

        private SolidColorBrush _bgColor;

        public SolidColorBrush BgColor
        {
            get => _bgColor;
            set
            {
                if (_bgColor == value) return;
                _bgColor = value;
                OnPropertyChanged(nameof(BgColor));
            }
        }

        #endregion
    }
}
