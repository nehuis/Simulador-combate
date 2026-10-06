namespace ProyectoSimulador.Personajes
{
    internal class Arquero : PersonajeBase
    {
        public Arquero(string nombre) : base(nombre, ataqueBase: 120, energiaMax: 70)
        {
        }

        public override bool UsarHabilidadEspecial()
        {
            if(EnergiaActual >= 25)
            {
                EnergiaActual -= 25;

                Console.WriteLine($"{Nombre} evitará el coontraataque de su enemigo");
                return true;
            }
            else
            {
                Console.WriteLine("Energía insuficiente");
                return false;
            }
        }
    }
}
