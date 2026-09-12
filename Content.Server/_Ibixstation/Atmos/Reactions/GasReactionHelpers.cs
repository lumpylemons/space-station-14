using Content.Shared._Ibixstation.Atmos.Prototypes;
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
        float lowestThreshold = Atmospherics.PlasmaMinimumBurnTemperature;

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
                if (gasProto.FuelProperties?.MinIgnitionTemperature < lowestThreshold)
                {
                    lowestThreshold = gasProto.FuelProperties?.MinIgnitionTemperature ?? lowestThreshold;
                }
            }
        }

        return lowestThreshold;
    }

    /// <summary>
    /// Resolves a single gas's color and opacity for a specific temperature.
    /// </summary>
    public static (Color Color, float Opacity) ResolveSingleGasVisuals(float currentTemperature, GasFuelProperties fuel)
    {
        var tempThresholds = fuel.FireVisualTempThresholds;

        if (tempThresholds == null || tempThresholds.Count == 0)
            return (Color.Orange, 0.8f);

        // Force baseline values if cooler than our minimum entry
        if (currentTemperature <= tempThresholds[0].Temperature)
            return (tempThresholds[0].Color, tempThresholds[0].Opacity);

        // Force maximum values if hotter than our highest milestone
        if (currentTemperature >= tempThresholds[tempThresholds.Count - 1].Temperature)
            return (tempThresholds[tempThresholds.Count - 1].Color, tempThresholds[tempThresholds.Count - 1].Opacity);

        // Bridge any two adjacent temperature points
        for (int i = 0; i < tempThresholds.Count - 1; i++)
        {
            var leftMarker = tempThresholds[i];
            var rightMarker = tempThresholds[i + 1];

            if (currentTemperature >= leftMarker.Temperature && currentTemperature <= rightMarker.Temperature)
            {
                float range = rightMarker.Temperature - leftMarker.Temperature;
                float progress = range > 0f ? (currentTemperature - leftMarker.Temperature) / range : 0f;

                Color blendedColor = Color.InterpolateBetween(leftMarker.Color, rightMarker.Color, progress);
                float blendedOpacity = leftMarker.Opacity + (rightMarker.Opacity - leftMarker.Opacity) * progress;

                return (blendedColor, Math.Clamp(blendedOpacity, 0f, 1f));
            }
        }

        return (tempThresholds[0].Color, tempThresholds[0].Opacity);
    }

    /// <summary>
    /// Combines all active gas parameters together based on their reacting mole quantities.
    /// </summary>
    public static Color CompositeMixedFireColor(ReadOnlySpan<(Color Color, float Opacity, float BurnedMoles)> resolvedGases)
    {
        if (resolvedGases.Length == 0)
            return Color.Orange;

        var sortedLayers = new List<(Color Color, float Opacity, float Moles)>(resolvedGases.Length);
        for (int i = 0; i < resolvedGases.Length; i++)
        {
            if (resolvedGases[i].BurnedMoles > 0f)
                sortedLayers.Add((resolvedGases[i].Color, resolvedGases[i].Opacity, resolvedGases[i].BurnedMoles));
        }

        if (sortedLayers.Count == 0)
            return Color.Orange;

        // Sort ascendingly by opacity so the thickest gas is calculated last as our base canvas
        sortedLayers.Sort((a, b) => a.Opacity.CompareTo(b.Opacity));

        Color finalHue = sortedLayers[0].Color;
        float maxOpacityFloor = sortedLayers[sortedLayers.Count - 1].Opacity;
        float runningTotalMoles = sortedLayers[0].Moles;

        // Processes multi-gas overlays cleanly if more than one fuel type is active
        for (int i = 1; i < sortedLayers.Count; i++)
        {
            var currentLayer = sortedLayers[i];
            runningTotalMoles += currentLayer.Moles;

            float blendWeight = currentLayer.Moles / runningTotalMoles;
            finalHue = Color.InterpolateBetween(finalHue, currentLayer.Color, blendWeight);
        }

        return finalHue.WithAlpha(maxOpacityFloor);
    }
}
