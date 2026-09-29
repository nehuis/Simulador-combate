using ProyectoSimulador.Interfaces;

namespace ProyectoSimulador.Personajes
{
    internal class Sacerdote : PersonajeBase, IRegenerable
    {
        public Sacerdote(string nombre) : base(nombre, ataqueBase: 105, energiaMax: 120)
        {
        }

        public override bool UsarHabilidadEspecial()
        {
            if(EnergiaActual >= 25)
            {
                EnergiaActual -= 25;

                if(VidaActual < 100)
                {
                    VidaActual += 30;

                    if(VidaActual > 100)
                    {
                        VidaActual = 100;
                    }
                }

                Console.WriteLine($"{Nombre} recuperó 30 puntos de vida");
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
            if (VidaActual > 0)
            {
                VidaActual += 10;
                Console.WriteLine($"Ronda finalizada. {Nombre} recupera 10 puntos de vida");
            }
        }
    }
}
