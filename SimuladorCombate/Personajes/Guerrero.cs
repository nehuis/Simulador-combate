using ProyectoSimulador.Interfaces;

namespace ProyectoSimulador.Personajes
{
    internal class Guerrero : PersonajeBase, IQuemadura
    {
        public Guerrero(string nombre) : base(nombre, ataqueBase: 125, energiaMax: 80)
        {
        }

        public override bool UsarHabilidadEspecial()
        {
            if (EnergiaActual >= 15)
            {
                EnergiaActual -= 15;

                if (VidaActual > 50)
                {
                    AtaqueBase = (int)(AtaqueBase * 1.25);
                    Console.WriteLine($"{Nombre} incrementó su ataque un 25% (Ataque actual: {AtaqueBase}).");
                }
                else
                {
                    AtaqueBase = (int)(AtaqueBase * 1.50);
                    Console.WriteLine($"{Nombre} (enfurecido) incrementó su ataque un 50% (Ataque actual: {AtaqueBase}).");
                }

                return true; 
            }

            Console.WriteLine("Energía insuficiente.");
            return false;
        }

        public void AplicarQuemadura(List<PersonajeBase> enemigos)
        {
            if (VidaActual <= 0) return;

            Console.WriteLine($"Ronda finalizada. {Nombre} inflige quemaduras a todos sus enemigos.");

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