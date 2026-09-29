using ProyectoSimulador.Interfaces;

namespace ProyectoSimulador.Personajes
{
    internal class Caballero : PersonajeBase, IRegenerable
    {
        public Caballero(string nombre) : base(nombre, ataqueBase: 115, energiaMax: 100)
        {
        }

        public override bool UsarHabilidadEspecial()
        {
            if(EnergiaActual >= 20)
            {
                EnergiaActual -= 20;
                DefensaBase = (int)(DefensaBase * 1.5);
                Console.WriteLine($"{Nombre} activó su escudo pesado");
                return true;
            }
            else
            {
                Console.WriteLine("Energía insuficiente");
                return false;
            }

        }

        public void Regenerar()
        {
            if(VidaActual > 0)
            {
                VidaActual += 10;
                Console.WriteLine($"Ronda finalizada. {Nombre} recupera 10 puntos de vida");
            }
        }
    }
}
