namespace RealtorObjects
{
    interface IDoubleNumericUpDown
    {
        CustomCommand IncreaseDouble {
            get;
        }
        CustomCommand DecreaseDouble {
            get;
        }
        void ChangeProperty(string name, double value);
    }
}
