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

                jugarNuevamente = ConsultarRespuestaSN("\n¿Desean jugar una revancha con los mismos personajes? (S/N): ");

                if (jugarNuevamente)
                {
                    ReiniciarEstadisticas();
                }
            }

            Console.WriteLine("\n¡Gracias por jugar!");
        }

        private void CargarJugadores()
        {
            _jugadores.Clear();
            bool cargar = true;

            while (cargar || _jugadores.Count < 2)
            {
                Console.Clear();
                Console.WriteLine("---- CARGANDO JUGADORES ----");
                Console.WriteLine($"Jugadores cargados: {_jugadores.Count}");

                string nombre = "";
                while (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.Write("Ingrese el nombre del jugador: ");
                    nombre = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(nombre))
                    {
                        Console.WriteLine("El nombre no puede estar vacío.");
                    }
                }

                Console.WriteLine("\nSeleccione un rol:");
                var roles = Enum.GetValues(typeof(EnumRol)).Cast<EnumRol>().ToList();
                foreach (var rol in roles)
                {
                    Console.WriteLine($"{(int)rol}. {rol}");
                }

                int opcionNum = LeerEnteroEnRango("Opción: ", (int)roles.Min(), (int)roles.Max());

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
                Console.WriteLine($"\n{nombre} ({nuevo.GetType().Name}) agregado con éxito.");

                if (_jugadores.Count < 2)
                {
                    Console.WriteLine("\nDebe haber al menos 2 jugadores para iniciar el juego.");
                    Console.WriteLine("Presione una tecla para cargar el siguiente...");
                    Console.ReadKey();
                }
                else
                {
                    cargar = ConsultarRespuestaSN("\n¿Desea ingresar otro jugador? (S/N): ");
                }
            }
        }

        private void EjecutarCombate()
        {
            int numeroRonda = 1;

            while (_jugadores.Count(p => p.VidaActual > 0) > 1)
            {
                Console.Clear();
                Console.WriteLine("====================");
                Console.WriteLine($"INICIO DE LA RONDA {numeroRonda}");
                Console.WriteLine("====================");

                foreach (var atacante in _jugadores)
                {
                    if (atacante.VidaActual > 0 && _jugadores.Count(p => p.VidaActual > 0) > 1)
                    {
                        EjecutarTurno(atacante);
                    }
                }

                FaseCierreRonda();

                numeroRonda++;
                Console.WriteLine("\nPresione una tecla para ir a la siguiente ronda...");
                Console.ReadKey();
            }

            var ganador = _jugadores.FirstOrDefault(p => p.VidaActual > 0);
            Console.Clear();
            Console.WriteLine("====================");
            Console.WriteLine(" ¡FIN DEL JUEGO! ");
            Console.WriteLine("====================");
            if (ganador != null)
            {
                Console.WriteLine($"El ganador es {ganador.Nombre}");
                Console.WriteLine($"Estadísticas finales -> Vida: {ganador.VidaActual} | Energía: {ganador.EnergiaActual}");
            }
        }

        private void EjecutarTurno(PersonajeBase atacante)
        {
            Console.WriteLine($"\n----------------------------------");
            Console.WriteLine($"TURNO DE: {atacante.Nombre} ({atacante.GetType().Name})");
            Console.WriteLine($"Vida: {atacante.VidaActual}/100 | Energía: {atacante.EnergiaActual}/{atacante.EnergiaMaxima}");

            bool quiereHabilidad = ConsultarRespuestaSN("¿Desea activar su Habilidad Especial? (S/N): ");
            bool habExitosa = false;

            if (quiereHabilidad)
            {
                habExitosa = atacante.UsarHabilidadEspecial();
            }

            bool esquivaContraataque = habExitosa && (atacante is Arquero);

            var enemigosVivos = _jugadores.Where(p => p != atacante && p.VidaActual > 0).ToList();
            Console.WriteLine("\nElija un enemigo para atacar:");

            for (int i = 0; i < enemigosVivos.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {enemigosVivos[i].Nombre} (Vida: {enemigosVivos[i].VidaActual})");
            }

            int opcionObjetivo = LeerEnteroEnRango("Opción: ", 1, enemigosVivos.Count);
            PersonajeBase objetivo = enemigosVivos[opcionObjetivo - 1];

            Console.WriteLine($"\n{atacante.Nombre} ataca a {objetivo.Nombre}");
            CalcularYAplicarDanio(atacante, objetivo);

            if (objetivo.VidaActual > 0 && !esquivaContraataque)
            {
                Console.WriteLine($"{objetivo.Nombre} contraataca a {atacante.Nombre}");
                CalcularYAplicarDanio(objetivo, atacante);
            }
            else if (esquivaContraataque)
            {
                Console.WriteLine($"¡{atacante.Nombre} evitó el contraataque gracias a su habilidad!");
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

        private void ReiniciarEstadisticas()
        {
            foreach (var p in _jugadores)
            {
                p.VidaActual = p.VidaMax;
                p.EnergiaActual = p.EnergiaMaxima;
                p.ResetearEstadisticasTemporales();
            }
            Console.WriteLine("\nTodas las estadísticas fueron restauradas a sus valores iniciales.");
        }

        private int LeerEnteroEnRango(string mensaje, int min, int max)
        {
            int valor;
            Console.Write(mensaje);
            while (!int.TryParse(Console.ReadLine(), out valor) || valor < min || valor > max)
            {
                Console.WriteLine($"Entrada inválida. Ingrese un número entre {min} y {max}.");
                Console.Write(mensaje);
            }
            return valor;
        }

        private bool ConsultarRespuestaSN(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                string entrada = Console.ReadLine()?.Trim().ToUpper();

                if (entrada == "S") return true;
                if (entrada == "N") return false;

                Console.WriteLine("Entrada inválida. Ingrese únicamente 'S' para Sí o 'N' para No.");
            }
        }
    }
}