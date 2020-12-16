namespace RealtorObjects
{
    interface IIntegerNumericUpDown
    {
        CustomCommand IncreaseInteger {
            get;
        }
        CustomCommand DecreaseInteger {
            get;
        }
        void ChangeProperty<T>(object obj, T gain);
    }
}
