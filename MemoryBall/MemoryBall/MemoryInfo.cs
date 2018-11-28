﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace MemoryBall
{
    public class MemoryInfo : INotifyPropertyChanged
    {
        private readonly double[] _table;
        private readonly int _r, _rR, _offset;

        public MemoryInfo()
        {
            _table = new[]
            {0, 0.06279052, 0.125333234, 0.187381315, 0.248689887, 0.309016994,
                0.368124553, 0.425779292, 0.481753674, 0.535826795, 0.587785252,
                0.63742399, 0.684547106, 0.728968627, 0.770513243, 0.809016994,
                0.844327926, 0.87630668, 0.904827052, 0.929776486, 0.951056516,
                0.968583161, 0.982287251, 0.992114701, 0.998026728, 1};
            _r = 26;
            _rR = 36;
            _offset = 38;
            _innerPoint = _outerPoint = _inner = _outer = new Point(38, 2);
        }

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InnerPoint"));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OuterPoint"));
            }
        }

        private string _memoryLoad;
        public string MemoryLoad
        {
            get => _memoryLoad;
            private set
            {
                if (_memoryLoad == value) return;
                _memoryLoad = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MemoryLoad"));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLargeArc"));
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
        #endregion

        #region 信息更新函数
        private void UpdateMemoryInfo()
        {
            MemoryLoad = $"{_memLoad.ToString()}%";
            if (_memLoad < 25)
            {
                IsLargeArc = false;
                _inner.X = _offset + _r * _table[_memLoad];
                _outer.X = _offset + _rR * _table[_memLoad];
                _inner.Y = _offset - _r * _table[25 - _memLoad];
                _outer.Y = _offset - _rR * _table[25 - _memLoad];
                InnerPoint = _inner;
                OuterPoint = _outer;
                return;
            }
            if (_memLoad < 50)
            {
                IsLargeArc = false;
                _inner.X = _offset + _r * _table[50 - _memLoad];
                _outer.X = _offset + _rR * _table[50 - _memLoad];
                _inner.Y = _offset + _r * _table[_memLoad - 25];
                _outer.Y = _offset + _rR * _table[_memLoad - 25];
                InnerPoint = _inner;
                OuterPoint = _outer;
                return;
            }
            if (_memLoad < 75)
            {
                IsLargeArc = true;
                _inner.X = _offset - _r * _table[_memLoad - 50];
                _outer.X = _offset - _rR * _table[_memLoad - 50];
                _inner.Y = _offset + _r * _table[75 - _memLoad];
                _outer.Y = _offset + _rR * _table[75 - _memLoad];
                InnerPoint = _inner;
                OuterPoint = _outer;
                return;
            }
            IsLargeArc = true;
            _inner.X = _offset - _r * _table[100 - _memLoad];
            _outer.X = _offset - _rR * _table[100 - _memLoad];
            _inner.Y = _offset - _r * _table[_memLoad - 75];
            _outer.Y = _offset - _rR * _table[_memLoad - 75];

            InnerPoint = _inner;
            OuterPoint = _outer;
        }

        #endregion
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
