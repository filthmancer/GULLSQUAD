namespace Gull
{
    public static class Posters
    {
        public static Tin.Events.EventRoutine<Gull.Events.EventBase> Input;
        public static Tin.Events.EventRoutine<Gull.Events.EventBase> Level;
    }
    public static partial class Events
    {
        public class EventBase
        {

        }
        static Events()
        {
            Initialise();
        }
        public static void Initialise()
        {
            Posters.Input = new Tin.Events.EventRoutine<Gull.Events.EventBase>();
            Posters.Level = new Tin.Events.EventRoutine<Gull.Events.EventBase>();
        }



    }
}