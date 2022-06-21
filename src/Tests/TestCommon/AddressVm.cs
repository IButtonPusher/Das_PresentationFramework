using System;
using Das.ViewModels;
// ReSharper disable All

namespace TestCommon
{
    public class AddressVm : BaseViewModel,
                             IEquatable<AddressVm>
    {


        private String _houseNumber;

        public String HouseNumber
        {
            get => _houseNumber;
            set => SetValue(ref _houseNumber, value);
        }
        
        private String _street;

        public String Street
        {
            get => _street;
            set => SetValue(ref _street, value);
        }


        private Int32 _zipCode;

        public Int32 ZipCode
        {
            get => _zipCode;
            set => SetValue(ref _zipCode, value);
        }


        private String _city;

        public String City
        {
            get => _city;
            set => SetValue(ref _city, value);
        }


        private String _state;

        public String State
        {
            get => _state;
            set => SetValue(ref _state, value);
        }

        public Boolean Equals(AddressVm other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _houseNumber == other._houseNumber && _street == other._street && _zipCode == other._zipCode && _city == other._city && _state == other._state;
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AddressVm) obj);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                var hashCode = (_houseNumber != null ? _houseNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_street != null ? _street.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _zipCode;
                hashCode = (hashCode * 397) ^ (_city != null ? _city.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_state != null ? _state.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
