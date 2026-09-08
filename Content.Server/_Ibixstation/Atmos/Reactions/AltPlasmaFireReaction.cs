using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class AltPlasmaFireReaction : IGasReactionEffect
    {
        public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
        {
            var dynamicIgnitionThreshold = GasReactionHelpers.GetDynamicIgnitionThreshold(mixture, atmosphereSystem);
            if (mixture.Temperature < dynamicIgnitionThreshold)
                return ReactionResult.NoReaction;

            var energyReleased = 0f;
            var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
            var temperature = mixture.Temperature;
            var location = holder as TileAtmosphere;
            mixture.ReactionResults[(byte)GasReaction.Fire] = 0;

            // More plasma released at higher temperatures.
            var temperatureScale = 0f;

            if (temperature > Atmospherics.PlasmaUpperTemperature)
                temperatureScale = 1f;
            else
            {
                temperatureScale = (temperature - Atmospherics.PlasmaMinimumBurnTemperature) /
                                   (Atmospherics.PlasmaUpperTemperature - Atmospherics.PlasmaMinimumBurnTemperature);
            }

            if (temperatureScale > 0)
            {
                var initialOxygenMoles = mixture.GetMoles(Gas.Oxygen);
                var initialPlasmaMoles = mixture.GetMoles(Gas.Plasma);

                if (initialPlasmaMoles <= 0f)
                    return ReactionResult.NoReaction;

                // Supersaturation makes hydrogen.
                var oxyRatio = initialOxygenMoles / initialPlasmaMoles;

                // Efficiency of reaction decreases based on oxygen-to-plasma ratio
                var supersaturation = Math.Clamp((oxyRatio - Atmospherics.SuperSaturationEnds) /
                                                 (Atmospherics.SuperSaturationThreshold -
                                                  Atmospherics.SuperSaturationEnds), 0.0f, 1.0f);

                // Stoichiometric O2 burn rate per plasma mole
                var oxygenBurnRate = IbixAtmospherics.PlasmaOxygenBurnRatioCarbonDioxide * (1.0f - supersaturation) +
                                     IbixAtmospherics.PlasmaOxygenBurnRatioTritium * supersaturation;

                var plasmaBurnRate = 0f;

                if (initialOxygenMoles > initialPlasmaMoles * Atmospherics.PlasmaOxygenFullburn)
                    plasmaBurnRate = initialPlasmaMoles * temperatureScale / IbixAtmospherics.PlasmaBurnRateDelta;
                else
                    plasmaBurnRate = temperatureScale * (initialOxygenMoles / Atmospherics.PlasmaOxygenFullburn) / IbixAtmospherics.PlasmaBurnRateDelta;

                if (plasmaBurnRate > Atmospherics.MinimumHeatCapacity)
                {
                    plasmaBurnRate = MathF.Min(plasmaBurnRate, MathF.Min(initialPlasmaMoles, initialOxygenMoles / oxygenBurnRate));

                    mixture.SetMoles(Gas.Plasma, initialPlasmaMoles - plasmaBurnRate);
                    mixture.SetMoles(Gas.Oxygen, initialOxygenMoles - (plasmaBurnRate * oxygenBurnRate));

                    // Plasma yields two products per mole burned which transmogrify to carbon or hydrogen
                    mixture.AdjustMoles(Gas.Hydrogen, plasmaBurnRate * 2f * supersaturation);
                    mixture.AdjustMoles(Gas.CarbonDioxide, plasmaBurnRate * 2f * (1.0f - supersaturation));

                    energyReleased += IbixAtmospherics.FirePlasmaEnergyReleased * plasmaBurnRate;
                    energyReleased /= heatScale; // adjust energy to make sure speedup doesn't cause mega temperature rise
                    mixture.ReactionResults[(byte)GasReaction.Fire] += plasmaBurnRate * (1.0f + oxygenBurnRate);
                }
            }

            if (energyReleased > 0)
            {
                var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
                if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
                    mixture.Temperature = (temperature * oldHeatCapacity + energyReleased) / newHeatCapacity;
            }

            if (location != null)
            {
                var mixTemperature = mixture.Temperature;
                if (mixTemperature > Atmospherics.FireMinimumTemperatureToExist)
                {
                    atmosphereSystem.HotspotExpose(location, mixTemperature, mixture.Volume);
                }
            }

            return mixture.ReactionResults[(byte)GasReaction.Fire] != 0 ? ReactionResult.Reacting : ReactionResult.NoReaction;
        }
    }
}
