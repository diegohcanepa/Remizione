using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// TargetAcquisition
    /// </summary>
    public static class TargetAcquisition
    {
        // FindBestTarget
        public static Actor? FindBestTarget(Actor self, float sightRange)
        {
            if (self.Room == null)
                return null;

            Actor? bestTarget = null;
            float closestDistanceSq = sightRange * sightRange;

            // Escaneamos todas las entidades activas en la habitación
            for (var i = 0; i < self.Room.CulledThings.Count; i++)
            {
                // Solo nos interesan entidades vivas
                if (self.Room.CulledThings[i] is Actor candidate)
                {
                    // Descartamos muertos y a sí mismo
                    if (candidate == self || candidate.IsDead)
                        continue;

                    // Si la entidad es invulnerable o intocable (ej. Vendedores), la ignoramos
                    if (!candidate.CanBeHit())
                        continue;

                    // Evaluamos con la Matriz si es un enemigo válido
                    if (!FactionMatrix.IsHostile(self, candidate))
                        continue;

                    // Chequeo de distancia (al cuadrado para ahorrar CPU)
                    float distSq = Vector2.DistanceSquared(self.Position, candidate.Position);
                    if (distSq > closestDistanceSq)
                        continue;

                    // Chequeo final de Line of Sight (para que no detecte al jugador a través de paredes)
                    if (!self.HasLineOfSightTo(candidate))
                        continue;

                    closestDistanceSq = distSq;
                    bestTarget = candidate;
                }
            }

            return bestTarget;
        }
    }
}