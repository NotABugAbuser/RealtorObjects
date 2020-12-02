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
        void ChangeProperty(string name, sbyte value);
    }
}
