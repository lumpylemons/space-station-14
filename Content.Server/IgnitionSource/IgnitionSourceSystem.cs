using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.IgnitionSource;

namespace Content.Server.IgnitionSource;

public sealed partial class IgnitionSourceSystem : SharedIgnitionSourceSystem
{
    [Dependency] private AtmosphereSystem _atmosphere = default!;
    [Dependency] private SharedTransformSystem _transform = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<IgnitionSourceComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var xform))
        {
            if (!comp.Ignited)
                continue;

            if (xform.GridUid is { } gridUid)
            {
                var position = _transform.GetGridOrMapTilePosition(uid, xform);
                // TODO: Should this be happening every single tick?
                // [Ibixstation] Handle above TODO to prevent fire colors being overridden.
                if (_atmosphere.IsHotspotActive(gridUid, position))
                {
                    var mixture = _atmosphere.GetTileMixture(gridUid, null, position);

                    if (mixture != null && mixture.Temperature >= comp.Temperature)
                        continue;
                }

                _atmosphere.HotspotExpose(gridUid, position, comp.Temperature, 50, uid, true);
            }
        }
    }
}
