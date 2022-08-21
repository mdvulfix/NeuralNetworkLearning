namespace APP.Brain
{
    public interface ISensible
    {
        Sensor Sensor {get; }
        void SetSensor(Sensor sensor);

    }
}