namespace Investry.Domain.Enums
{
    public enum SystemTransactionType
    {
        EscrowHold,       // لما المستثمر يستثمر
        EscrowRelease,    // لما الحملة تنجح → تتحول للـ Founder
        EscrowRefund,     // لما الحملة تفشل → ترجع للمستثمر
        PlatformFeeCredit // عمولة المنصة
    }
}
