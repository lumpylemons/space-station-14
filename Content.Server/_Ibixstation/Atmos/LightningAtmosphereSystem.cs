using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;

namespace Content.Server._Ibixstation.Atmos;

public sealed partial class LightningAtmosphereSystem : EntitySystem
{
    [Dependency] private AtmosphereSystem _atmosphereSystem = default!;

    public void ProcessLightningPlasmaReaction(GasMixture mixture, EntityUid? gridUid, Vector2i tileIndices, float strikeDamage)
    {
        var nTotal = mixture.TotalMoles;
        if (nTotal <= Atmospherics.GasMinMoles)
            return;

        var energyScale = MathF.Max(1.0f, strikeDamage / IbixAtmospherics.LightningBaseEnergyScaleDivisor);
        var currentTemp = mixture.Temperature;

        var totalOxidizerPower = 0f;
        var hasFuel = false;

        for (var i = 0; i < Atmospherics.TotalNumberOfGases; i++)
        {
            var moles = mixture.GetMoles(i);
            if (moles < Atmospherics.GasMinMoles)
                continue;

            var gasProto = _atmosphereSystem.GetGas(i);

            if (gasProto.IsOxidizer && gasProto.OxidizerProperties != null)
            {
                totalOxidizerPower += moles * gasProto.OxidizerProperties.Value.Power;
            }

            if (gasProto.IsFuel)
            {
                hasFuel = true;
            }
        }

        if (totalOxidizerPower > Atmospherics.GasMinMoles && hasFuel)
        {
            mixture.Temperature = MathF.Max(mixture.Temperature, Atmospherics.PlasmaMinimumBurnTemperature * 1.5f);

            if (gridUid != null)
            {
                _atmosphereSystem.HotspotExpose(gridUid.Value, tileIndices, mixture.Temperature, mixture.Volume, color: Color.Orange);
            }
            return;
        }

        var nO2 = mixture.GetMoles(Gas.Oxygen);
        var fO2 = nO2 / nTotal;

        if (nO2 > Atmospherics.GasMinMoles && currentTemp < IbixAtmospherics.LightningOzoneThermalCeiling)
        {
            var ozoneDelta = (nO2 / IbixAtmospherics.LightningOzoneReactionRate) * fO2 * energyScale;
            ozoneDelta = MathF.Min(ozoneDelta, nO2);

            mixture.AdjustMoles(Gas.Oxygen, -ozoneDelta);
            mixture.AdjustMoles(Gas.Ozone, ozoneDelta);

            nO2 = mixture.GetMoles(Gas.Oxygen);
            fO2 = nO2 / nTotal;
        }

        var nN2 = mixture.GetMoles(Gas.Nitrogen);
        if (nN2 > Atmospherics.GasMinMoles && nO2 > Atmospherics.GasMinMoles)
        {
            var fN2 = nN2 / nTotal;
            var collisionRate = fN2 * fO2;
            var fixationDelta = (nN2 / IbixAtmospherics.LightningFixationReactionRate) * collisionRate * energyScale;
            var maxFixationByOxygen = nO2 / 0.5f;
            fixationDelta = MathF.Min(fixationDelta, MathF.Min(nN2, maxFixationByOxygen));

            if (fixationDelta > Atmospherics.GasMinMoles)
            {
                mixture.AdjustMoles(Gas.Nitrogen, -fixationDelta);
                mixture.AdjustMoles(Gas.Oxygen, -(fixationDelta * 0.5f));
                mixture.AdjustMoles(Gas.NitrousOxide, fixationDelta);

                nO2 = mixture.GetMoles(Gas.Oxygen);
                fO2 = nO2 / nTotal;
            }
        }

        var nCO2 = mixture.GetMoles(Gas.CarbonDioxide);
        if (nCO2 > Atmospherics.GasMinMoles)
        {
            var fCO2 = nCO2 / nTotal;
            var coDelta = (nCO2 / IbixAtmospherics.LightningCarbonDissociationRate) * fCO2 * energyScale;
            coDelta = MathF.Min(coDelta, nCO2);

            mixture.AdjustMoles(Gas.CarbonDioxide, -coDelta);
            mixture.AdjustMoles(Gas.CarbonMonoxide, coDelta);
            mixture.AdjustMoles(Gas.Oxygen, coDelta * 0.5f);
        }

        var nWater = mixture.GetMoles(Gas.WaterVapor);
        if (nWater > Atmospherics.GasMinMoles)
        {
            var fWater = nWater / nTotal;
            var waterDelta = (nWater / IbixAtmospherics.LightningWaterSplittingRate) * fWater * energyScale;
            waterDelta = MathF.Min(waterDelta, nWater);

            mixture.AdjustMoles(Gas.WaterVapor, -waterDelta);
            mixture.AdjustMoles(Gas.Hydrogen, waterDelta);
            mixture.AdjustMoles(Gas.Oxygen, waterDelta * 0.5f);
        }
    }
}
