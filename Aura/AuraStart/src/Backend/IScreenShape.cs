using System;
using System.Numerics;

namespace Aura.Veldrid
{
    public interface IScreenShape
    {
        Vector2 ConvertMouseToAura(Vector2 mouse);
        void SetViewAt(Vector2 auraPos);
    }
}
