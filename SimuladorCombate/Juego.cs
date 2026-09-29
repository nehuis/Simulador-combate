using ProyectoSimulador.Interfaces;
using ProyectoSimulador.Personajes;

namespace ProyectoSimulador
{
    internal class Juego
    {
        private List<PersonajeBase> _jugadores = new List<PersonajeBase>();

        public void Iniciar()
        {
            bool jugarNuevamente = true;

            CargarJugadores();

            while (jugarNuevamente)
            {
                EjecutarCombate();

                jugarNuevamente = ConsultarRevancha();

                if (jugarNuevamente)
                {
                    ReiniciarEstadisticas();
                }
            }

            Console.WriteLine("\n ¡Gracias por jugar!");
        }
        private void CargarJugadores()
        {
            bool cargar = true;

            while(cargar || _jugadores.Count < 2)
            {
                Console.Clear();
                Console.WriteLine("---- CARGANDO JUGADORES ----");
                Console.WriteLine($"Jugadores cargados : {_jugadores.Count}");

                Console.Write("Ingrese el nombre del jugador: ");
                string nombre = Console.ReadLine();

                Console.WriteLine("\n Seleccione un rol: ");
                foreach(var rol in Enum.GetValues(typeof(EnumRol)))
                {
                    Console.WriteLine($"{(int)rol}. {rol}");
                }

                Console.Write("Opción: ");
                int.TryParse(Console.ReadLine(), out int opcionNum);

                PersonajeBase nuevo = (EnumRol)opcionNum switch
                {
                    EnumRol.Caballero => new Caballero(nombre),
                    EnumRol.Guerrero => new Guerrero(nombre),
                    EnumRol.Mago => new Mago(nombre),
                    EnumRol.Sacerdote => new Sacerdote(nombre),
                    EnumRol.Arquero => new Arquero(nombre),
                    _ => new Caballero(nombre)
                };

                _jugadores.Add(nuevo);
                Console.WriteLine($"\n{nombre} ({nuevo.GetType().Name}) agregado con éxito");

                if (_jugadores.Count < 2)
                {
                    Console.WriteLine("\n Debe haber al menos 2 jugadores para iniciar. Presione una tecla para cargar el siguiente...");
                    Console.ReadKey();
                }
                else
                {
                    Console.Write("\n¿Desea ingresar otro jugador? (S/N): ");
                    cargar = Console.ReadLine()?.Trim().ToUpper() == "S";
                }
            }
        }

        private void EjecutarCombate()
        {
            int numeroRonda = 1;

            while(_jugadores.Count(p => p.VidaActual > 0) > 1)
            {
                Console.Clear();
                Console.WriteLine("====================");
                Console.WriteLine($"INICIO DE LA RONDA {numeroRonda}");
                Console.WriteLine("====================");

                foreach(var atacante in _jugadores)
                {
                    if(atacante.VidaActual > 0 && _jugadores.Count(p => p.VidaActual > 0) > 1)
                    {
                        EjecutarTurno(atacante);
                    }
                }

                FaseCierreRonda();

                numeroRonda++;
                Console.WriteLine("\n Presione una tecla para ir a la siguiente ronda");
                Console.ReadKey();
            }

            var ganador = _jugadores.FirstOrDefault(p => p.VidaActual > 0);
            Console.Clear();
            Console.WriteLine("¡FIN DEL JUEGO!");
            Console.WriteLine($"El ganador es {ganador.Nombre}");
            Console.WriteLine($"Estadísticas finales -> Vida: {ganador.VidaActual} | Energía: {ganador.EnergiaActual}");
        }

        private void EjecutarTurno(PersonajeBase atacante)
        {
            Console.WriteLine($"\n----------------------------------");
            Console.WriteLine($"TURNO DE: {atacante.Nombre}");
            Console.WriteLine($"Vida: {atacante.VidaActual}/100 | Energía: {atacante.EnergiaActual}/{atacante.EnergiaMaxima}");

            Console.Write("¿Desea activar su Habilidad Especial? (S/N): ");
            bool quiereHabilidad = Console.ReadLine()?.Trim().ToUpper() == "S";
            bool esquivaContraataque = false;

            if (quiereHabilidad)
            {
                esquivaContraataque = atacante.UsarHabilidadEspecial();
            }

            var enemigosVivos = _jugadores.Where(p => p != atacante && p.VidaActual > 0).ToList();
            Console.WriteLine("\n Elija un enemigo para atacar");

            for(int i = 0; i < enemigosVivos.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {enemigosVivos[i].Nombre} (Vida: {enemigosVivos[i].VidaActual})");
            }

            Console.Write("Opción: ");
            int.TryParse(Console.ReadLine(), out int opcionObjetivo);

            int indice = Math.Max(0, Math.Min(opcionObjetivo - 1, enemigosVivos.Count - 1));
            PersonajeBase objetivo = enemigosVivos[indice];

            Console.WriteLine($"\n {atacante.Nombre} ataca a {objetivo.Nombre}");
            CalcularYAplicarDanio(atacante, objetivo);

            if (objetivo.VidaActual > 0 && !esquivaContraataque)
            {
                Console.WriteLine($"{objetivo.Nombre} contraataca a {atacante.Nombre}");
                CalcularYAplicarDanio(objetivo, atacante);
            }
            else if (esquivaContraataque)
            {
                Console.WriteLine($"{atacante.Nombre} anuló el contraataque");
            }

            atacante.ResetearEstadisticasTemporales();
        }

        private void CalcularYAplicarDanio(PersonajeBase atacante, PersonajeBase defensor)
        {
            int danio = Math.Max(1, atacante.AtaqueBase - defensor.DefensaBase);
            defensor.VidaActual = Math.Max(0, defensor.VidaActual - danio);

            Console.WriteLine($"   -> Inflige {danio} de daño. Vida de {defensor.Nombre}: {defensor.VidaActual}");

            if (defensor.VidaActual == 0)
            {
                Console.WriteLine($"{defensor.Nombre} ha muerto");
            }
        }

        private void FaseCierreRonda()
        {
            Console.WriteLine("\n--- CIERRE DE RONDA ---");

            foreach (var p in _jugadores.OfType<IRegenerable>())
            {
                p.Regenerar();
            }

            foreach (var p in _jugadores.OfType<IQuemadura>())
            {
                p.AplicarQuemadura(_jugadores);
            }
        }

        private bool ConsultarRevancha()
        {
            Console.Write("\n¿Desean jugar una revancha con los mismos personajes? (S/N): ");
            return Console.ReadLine()?.Trim().ToUpper() == "S";
        }

        private void ReiniciarEstadisticas()
        {
            foreach (var p in _jugadores)
            {
                p.VidaActual = 100;
                p.EnergiaActual = p.EnergiaMaxima;
                p.ResetearEstadisticasTemporales();
            }
            Console.WriteLine("\nTodas las estadísticas fueron restauradas a sus valores iniciales");
        }
    }
}
