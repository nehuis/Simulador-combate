using ProyectoSimulador.Interfaces;

namespace ProyectoSimulador.Personajes
{
    internal class Mago : PersonajeBase, IQuemadura
    {
        public Mago(string nombre) : base(nombre, ataqueBase: 135, energiaMax: 90)
        {
        }

        public override bool UsarHabilidadEspecial()
        {
            if(EnergiaMaxima >= 40)
            {
                EnergiaActual -= 40;
                AtaqueBase = (int)(AtaqueBase * 1.5);
                Console.WriteLine($"{Nombre} activó su hechizo devastador");
                return true;
            }
            else
            {
                Console.WriteLine("Energía insuficiente");
                return false;
            }

        }

        public void AplicarQuemadura(List<PersonajeBase> enemigos)
        {
            if (VidaActual > 0)
            {
                Console.WriteLine($"Ronda finalizada. {Nombre} inflige quemaduras a todos sus enemigos");

                foreach (var enemigo in enemigos)
                {
                    if (enemigo != this && enemigo.VidaActual > 0)
                    {
                        enemigo.VidaActual = Math.Max(0, enemigo.VidaActual - 5);
                        Console.WriteLine($"{enemigo.Nombre} recibió 5 puntos de daño por quemadura. Vida actual: {enemigo.VidaActual}");
                    }
                }
            }
        }
    }
}
