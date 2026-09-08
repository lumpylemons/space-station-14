using Content.Shared.Atmos;
using Content.Shared.Atmos.EntitySystems;

namespace Content.Server._Ibixstation.Atmos.Reactions;

public static class GasReactionHelpers
{
    /// <summary>
    /// Evaluates a gas mixture to determine the dynamic minimum ignition temperature
    /// based on the easiest-to-ignite fuel currently present in viable quantities.
    /// </summary>
    public static float GetDynamicIgnitionThreshold(GasMixture mixture, SharedAtmosphereSystem atmosSystem)
    {
        float lowestThreshold = float.MaxValue;
        bool fuelFound = false;

        // Iterate through all possible gases using the public accessor
        for (int i = 0; i < Atmospherics.TotalNumberOfGases; i++)
        {
            var gasEnum = (Gas)i;

            if (mixture.GetMoles(gasEnum) <= Atmospherics.GasMinMoles)
                continue;

            var gasProto = atmosSystem.GetGas(i);

            // Check if the gas is a reactive fuel profile
            if (gasProto.IsFuel)
            {
                fuelFound = true;
                if (gasProto.MinIgnitionTemperature < lowestThreshold)
                {
                    lowestThreshold = gasProto.MinIgnitionTemperature;
                }
            }
        }

        // Return easiest barrier to break, or fall back to system default if for some reason no fuel is found
        return fuelFound ? lowestThreshold : Atmospherics.PlasmaMinimumBurnTemperature;
    }
}

