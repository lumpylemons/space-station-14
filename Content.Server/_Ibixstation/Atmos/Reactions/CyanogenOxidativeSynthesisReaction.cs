using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class CyanogenOxidativeSynthesisReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var nMethane = mixture.GetMoles(Gas.Methane);
        var nAmmonia = mixture.GetMoles(Gas.Ammonia);
        var nOxygen = mixture.GetMoles(Gas.Oxygen);
        var nTotal = mixture.TotalMoles;

        var maxByMethane = nMethane / 2f;
        var maxByAmmonia = nAmmonia / 2f;
        var maxByOxygen = nOxygen / 2f;

        var reactingUnits = MathF.Min(maxByMethane, MathF.Min(maxByAmmonia, maxByOxygen));

        var fMethane = nMethane / nTotal;
        var fAmmonia = nAmmonia / nTotal;
        var fOxygen = nOxygen / nTotal;

        var limitingFraction = MathF.Min(fMethane, MathF.Min(fAmmonia, fOxygen));
        var tempFactor = MathF.Min(2.5f, mixture.Temperature / 1200f);

        var rate = limitingFraction * tempFactor;
        var deltaCyanogen = (reactingUnits / IbixAtmospherics.CyanogenOxidativeReactionRate) * rate;

        var deltaMethane = deltaCyanogen * 2f;
        var deltaAmmonia = deltaCyanogen * 2f;
        var deltaOxygen = deltaCyanogen * 2f;
        var deltaWater = deltaCyanogen * 6f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.Methane, -deltaMethane);
        mixture.AdjustMoles(Gas.Ammonia, -deltaAmmonia);
        mixture.AdjustMoles(Gas.Oxygen, -deltaOxygen);
        mixture.AdjustMoles(Gas.Cyanogen, deltaCyanogen);
        mixture.AdjustMoles(Gas.WaterVapor, deltaWater);

        var energyReleased = deltaCyanogen * IbixAtmospherics.CyanogenOxidativeEnergyReleased;
        energyReleased /= heatScale;

        if (energyReleased > 0f)
        {
            var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
            if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
            {
                mixture.Temperature = (mixture.Temperature * oldHeatCapacity + energyReleased) / newHeatCapacity;
            }
        }

        return ReactionResult.Reacting;
    }
}
