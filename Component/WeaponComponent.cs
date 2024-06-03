namespace EC.Component
{
    public enum WeaponType
    {
        None,
        Unarmed,
    }

    public class WeaponComponent : EComponent
    {
        public WeaponType HoldingWeapon
        {
            get;
            private set;
        }

        public WeaponComponent(): base(ComponentType.Weapon)
        {
        }

        public override void Init()
        {
        }

        public void SetWeapon(WeaponType type)
        {
            HoldingWeapon = type;
        }

        public override void Dispose()
        {
        }
    }
}