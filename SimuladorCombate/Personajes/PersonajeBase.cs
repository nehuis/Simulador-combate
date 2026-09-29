namespace ProyectoSimulador.Personajes
{
    internal abstract class PersonajeBase
    {
        public string Nombre { get; set; }
        public int VidaActual { get; set; }
        public int VidaMax { get; protected set; } = 100;

        public int AtaqueBase { get; set; }
        public int DefensaBase { get; set; }

        public int EnergiaActual { get; set; }
        public int EnergiaMaxima { get; protected set; }

        private readonly int _ataqueInicial;
        private readonly int _defensaInicial;

        public PersonajeBase(string nombre, int ataqueBase, int energiaMax, int defensaBase = 0)
        {
            Nombre = nombre;
            VidaActual = VidaMax;

            AtaqueBase = ataqueBase;
            DefensaBase = defensaBase;

            EnergiaMaxima = energiaMax;
            EnergiaActual = energiaMax;

            _ataqueInicial = ataqueBase;
            _defensaInicial = defensaBase;
        }

        public void ResetearEstadisticasTemporales()
        {
            AtaqueBase = _ataqueInicial;
            DefensaBase = _defensaInicial;
        }

        public abstract bool UsarHabilidadEspecial();
    }
}