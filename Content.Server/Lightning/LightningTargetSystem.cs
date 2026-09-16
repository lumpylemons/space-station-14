using Content.Server._Ibixstation.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Lightning;
using Content.Server.Lightning.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Robust.Server.GameObjects;

namespace Content.Server.Tesla.EntitySystems;

/// <summary>
/// The component allows lightning to strike this target. And determining the behavior of the target when struck by lightning.
/// </summary>
public sealed partial class LightningTargetSystem : EntitySystem
{
    [Dependency] private DamageableSystem _damageable = default!;
    [Dependency] private ExplosionSystem _explosionSystem = default!;
    [Dependency] private TransformSystem _transform = default!;
    [Dependency] private AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private LightningAtmosphereSystem _lightningAtmos = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<LightningTargetComponent, HitByLightningEvent>(OnHitByLightning);
    }

    private void OnHitByLightning(Entity<LightningTargetComponent> uid, ref HitByLightningEvent args)
    {
        DamageSpecifier damage = new();
        damage.DamageDict.Add("Structural", uid.Comp.DamageFromLightning);
        _damageable.ChangeDamage(uid.Owner, damage, true);

        // [Ibixstation]
        if (TryComp<TransformComponent>(uid, out var xform))
        {
            var entityTransform = new Entity<TransformComponent?>(uid.Owner, xform);

            if (_atmosphereSystem.GetTileMixture(entityTransform) is { } mixture)
            {
                var gridUid = xform.GridUid;
                var tileIndices = _transform.GetGridTilePositionOrDefault((uid.Owner, xform));

                _lightningAtmos.ProcessLightningPlasmaReaction(mixture, gridUid, tileIndices, (float)uid.Comp.DamageFromLightning);
            }
        }
        // [Ibixstation] end

        if (uid.Comp.LightningExplode)
        {
            _explosionSystem.QueueExplosion(
                _transform.GetMapCoordinates(uid),
                uid.Comp.ExplosionPrototype,
                uid.Comp.TotalIntensity, uid.Comp.Dropoff,
                uid.Comp.MaxTileIntensity,
                uid,
                canCreateVacuum: false);
        }
    }
}
