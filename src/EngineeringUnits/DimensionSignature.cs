using System;

namespace EngineeringUnits
{
    /// <summary>
    /// The physical dimension of a quantity, expressed as a signed exponent for each
    /// SI base dimension plus a pseudo-dimension for plane angle (radians).
    /// </summary>
    /// <remarks>
    /// Eight slots in total: length (L), mass (M), time (T), electric current (I),
    /// thermodynamic temperature (Θ), amount of substance (N), luminous intensity (J),
    /// and plane angle (A). The angle slot is a deliberate non-SI addition: it lets the
    /// library distinguish frequency (1/s) from angular velocity (rad/s), and linear
    /// power from rotational power. See specification §14.15.
    /// </remarks>
    public readonly struct DimensionSignature : IEquatable<DimensionSignature>
    {
        /// <summary>Length exponent.</summary>
        public sbyte L { get; }
        /// <summary>Mass exponent.</summary>
        public sbyte M { get; }
        /// <summary>Time exponent.</summary>
        public sbyte T { get; }
        /// <summary>Electric current exponent.</summary>
        public sbyte I { get; }
        /// <summary>Thermodynamic temperature exponent.</summary>
        public sbyte Theta { get; }
        /// <summary>Amount of substance exponent.</summary>
        public sbyte N { get; }
        /// <summary>Luminous intensity exponent.</summary>
        public sbyte J { get; }
        /// <summary>Plane angle (radian) exponent — a pseudo-dimension; see remarks on the type.</summary>
        public sbyte A { get; }

        /// <summary>Constructs a dimension signature. All exponents default to zero.</summary>
        public DimensionSignature(
            sbyte L = 0, sbyte M = 0, sbyte T = 0, sbyte I = 0,
            sbyte Theta = 0, sbyte N = 0, sbyte J = 0, sbyte A = 0)
        {
            this.L = L; this.M = M; this.T = T; this.I = I;
            this.Theta = Theta; this.N = N; this.J = J; this.A = A;
        }

        /// <summary>True if every exponent is zero (a pure number).</summary>
        public bool IsDimensionless =>
            L == 0 && M == 0 && T == 0 && I == 0 &&
            Theta == 0 && N == 0 && J == 0 && A == 0;

        /// <summary>The dimensionless signature (all zeros).</summary>
        public static readonly DimensionSignature Dimensionless = default;

        /// <summary>Length.</summary>
        public static readonly DimensionSignature Length = new DimensionSignature(L: 1);
        /// <summary>Mass.</summary>
        public static readonly DimensionSignature Mass = new DimensionSignature(M: 1);
        /// <summary>Time.</summary>
        public static readonly DimensionSignature Time = new DimensionSignature(T: 1);
        /// <summary>Thermodynamic temperature.</summary>
        public static readonly DimensionSignature Temperature = new DimensionSignature(Theta: 1);
        /// <summary>Electric current.</summary>
        public static readonly DimensionSignature ElectricCurrent = new DimensionSignature(I: 1);
        /// <summary>Amount of substance.</summary>
        public static readonly DimensionSignature AmountOfSubstance = new DimensionSignature(N: 1);
        /// <summary>Luminous intensity.</summary>
        public static readonly DimensionSignature LuminousIntensity = new DimensionSignature(J: 1);
        /// <summary>Plane angle.</summary>
        public static readonly DimensionSignature PlaneAngle = new DimensionSignature(A: 1);

        /// <summary>Element-wise addition (used to compose dimensions through multiplication).</summary>
        public static DimensionSignature operator +(DimensionSignature a, DimensionSignature b) =>
            new DimensionSignature(
                (sbyte)(a.L + b.L), (sbyte)(a.M + b.M), (sbyte)(a.T + b.T), (sbyte)(a.I + b.I),
                (sbyte)(a.Theta + b.Theta), (sbyte)(a.N + b.N), (sbyte)(a.J + b.J), (sbyte)(a.A + b.A));

        /// <summary>Element-wise subtraction (used to compose dimensions through division).</summary>
        public static DimensionSignature operator -(DimensionSignature a, DimensionSignature b) =>
            new DimensionSignature(
                (sbyte)(a.L - b.L), (sbyte)(a.M - b.M), (sbyte)(a.T - b.T), (sbyte)(a.I - b.I),
                (sbyte)(a.Theta - b.Theta), (sbyte)(a.N - b.N), (sbyte)(a.J - b.J), (sbyte)(a.A - b.A));

        /// <summary>Element-wise unary negation (used for reciprocal dimensions like 1/Time).</summary>
        public static DimensionSignature operator -(DimensionSignature a) =>
            new DimensionSignature(
                (sbyte)(-a.L), (sbyte)(-a.M), (sbyte)(-a.T), (sbyte)(-a.I),
                (sbyte)(-a.Theta), (sbyte)(-a.N), (sbyte)(-a.J), (sbyte)(-a.A));

        /// <summary>Element-wise multiplication by an integer (used to raise a quantity to a power).</summary>
        public static DimensionSignature operator *(DimensionSignature a, int factor) =>
            new DimensionSignature(
                (sbyte)(a.L * factor), (sbyte)(a.M * factor), (sbyte)(a.T * factor), (sbyte)(a.I * factor),
                (sbyte)(a.Theta * factor), (sbyte)(a.N * factor), (sbyte)(a.J * factor), (sbyte)(a.A * factor));

        public bool Equals(DimensionSignature other) =>
            L == other.L && M == other.M && T == other.T && I == other.I &&
            Theta == other.Theta && N == other.N && J == other.J && A == other.A;

        public override bool Equals(object? obj) => obj is DimensionSignature other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + L;  hash = hash * 31 + M;
                hash = hash * 31 + T;  hash = hash * 31 + I;
                hash = hash * 31 + Theta; hash = hash * 31 + N;
                hash = hash * 31 + J;  hash = hash * 31 + A;
                return hash;
            }
        }

        public static bool operator ==(DimensionSignature a, DimensionSignature b) => a.Equals(b);
        public static bool operator !=(DimensionSignature a, DimensionSignature b) => !a.Equals(b);

        /// <summary>
        /// A compact human-readable form like <c>L*M/T^2</c> for force. Uses caret notation
        /// regardless of preferences (preferences only affect <see cref="EngineeringUnit"/>
        /// display, not this debug-style projection).
        /// </summary>
        public override string ToString()
        {
            if (IsDimensionless) return "1";
            var pos = new System.Collections.Generic.List<string>();
            var neg = new System.Collections.Generic.List<string>();
            void Add(string name, sbyte exp)
            {
                if (exp == 0) return;
                var token = exp == 1 || exp == -1 ? name : $"{name}^{System.Math.Abs((int)exp)}";
                (exp > 0 ? pos : neg).Add(token);
            }
            Add("L", L); Add("M", M); Add("T", T); Add("I", I);
            Add("Θ", Theta); Add("N", N); Add("J", J); Add("A", A);
            var posStr = pos.Count == 0 ? "1" : string.Join("*", pos);
            var negStr = neg.Count == 0 ? "" : "/" + string.Join("/", neg);
            return posStr + negStr;
        }
    }
}
