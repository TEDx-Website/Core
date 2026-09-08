export function HeroSparks() {
  const sparks = [
    {
      id: 1,
      t: "10%",
      l: "15%",
      s: "w-2 h-2",
      c: "bg-white/60",
      d: "0s",
      b: "blur-[1px]",
      sh: "shadow-[0_0_12px_rgba(255,255,255,0.8)]",
    },
    {
      id: 2,
      t: "25%",
      l: "75%",
      s: "w-3 h-3",
      c: "bg-brand-500/70",
      d: "1.5s",
      b: "blur-[2px]",
      sh: "shadow-[0_0_15px_rgba(235,0,40,0.8)]",
    },
    {
      id: 3,
      t: "45%",
      l: "10%",
      s: "w-1.5 h-1.5",
      c: "bg-brand-glow/80",
      d: "3s",
      b: "blur-[1px]",
      sh: "shadow-[0_0_15px_rgba(255,43,68,0.8)]",
    },
    {
      id: 4,
      t: "70%",
      l: "85%",
      s: "w-2.5 h-2.5",
      c: "bg-white/50",
      d: "0.5s",
      b: "blur-[2px]",
      sh: "shadow-[0_0_10px_rgba(255,255,255,0.6)]",
    },
    {
      id: 5,
      t: "80%",
      l: "20%",
      s: "w-3 h-3",
      c: "bg-brand-500/60",
      d: "2.5s",
      b: "blur-[1.5px]",
      sh: "shadow-[0_0_12px_rgba(235,0,40,0.6)]",
    },
    {
      id: 6,
      t: "15%",
      l: "50%",
      s: "w-2 h-2",
      c: "bg-brand-glow/70",
      d: "1s",
      b: "blur-[1px]",
      sh: "shadow-[0_0_10px_rgba(255,43,68,0.7)]",
    },
    {
      id: 7,
      t: "55%",
      l: "60%",
      s: "w-1 h-1",
      c: "bg-white/70",
      d: "4s",
      b: "blur-[0.5px]",
      sh: "shadow-[0_0_8px_rgba(255,255,255,0.8)]",
    },
    {
      id: 8,
      t: "85%",
      l: "65%",
      s: "w-2.5 h-2.5",
      c: "bg-brand-500/80",
      d: "2s",
      b: "blur-[2px]",
      sh: "shadow-[0_0_15px_rgba(235,0,40,0.9)]",
    },
    {
      id: 9,
      t: "35%",
      l: "85%",
      s: "w-2 h-2",
      c: "bg-brand-glow/60",
      d: "0.8s",
      b: "blur-[1.5px]",
      sh: "shadow-[0_0_12px_rgba(255,43,68,0.6)]",
    },
    {
      id: 10,
      t: "5%",
      l: "80%",
      s: "w-1.5 h-1.5",
      c: "bg-white/50",
      d: "3.5s",
      b: "blur-[1px]",
      sh: "shadow-[0_0_10px_rgba(255,255,255,0.5)]",
    },
    {
      id: 11,
      t: "65%",
      l: "25%",
      s: "w-3 h-3",
      c: "bg-brand-500/50",
      d: "1.2s",
      b: "blur-[2.5px]",
      sh: "shadow-[0_0_12px_rgba(235,0,40,0.5)]",
    },
    {
      id: 12,
      t: "90%",
      l: "10%",
      s: "w-2 h-2",
      c: "bg-brand-glow/75",
      d: "4.5s",
      b: "blur-[1px]",
      sh: "shadow-[0_0_14px_rgba(255,43,68,0.7)]",
    },
    {
      id: 13,
      t: "40%",
      l: "40%",
      s: "w-1.5 h-1.5",
      c: "bg-white/60",
      d: "2.2s",
      b: "blur-[1px]",
      sh: "shadow-[0_0_10px_rgba(255,255,255,0.6)]",
    },
    {
      id: 14,
      t: "75%",
      l: "45%",
      s: "w-2 h-2",
      c: "bg-brand-500/65",
      d: "0.3s",
      b: "blur-[1.5px]",
      sh: "shadow-[0_0_12px_rgba(235,0,40,0.6)]",
    },
    {
      id: 15,
      t: "20%",
      l: "30%",
      s: "w-2.5 h-2.5",
      c: "bg-brand-glow/85",
      d: "3.2s",
      b: "blur-[2px]",
      sh: "shadow-[0_0_16px_rgba(255,43,68,0.8)]",
    },
    {
      id: 16,
      t: "50%",
      l: "90%",
      s: "w-1 h-1",
      c: "bg-white/80",
      d: "1.8s",
      b: "blur-[0.5px]",
      sh: "shadow-[0_0_8px_rgba(255,255,255,0.9)]",
    },
  ];

  return (
    <div
      className="absolute inset-0 w-full h-full pointer-events-none overflow-hidden z-0"
      style={{ perspective: "1200px" }}
    >
      <div
        className="absolute inset-0 w-full h-full"
        style={{ transformStyle: "preserve-3d" }}
      >
        <div
          className="absolute top-1/2 left-1/2 w-[150vw] h-[150vw] bg-[radial-gradient(circle_at_center,rgba(235,0,40,0.15)_0%,rgba(235,0,40,0.05)_30%,transparent_70%)] blur-[60px] mix-blend-screen"
          style={{ transform: "translate3d(-50%, -50%, -400px)" }}
        />
        <div
          className="absolute top-1/2 left-1/2 w-[30vw] h-[60vh] min-w-75 min-h-125 border-2 border-white/5 shadow-[inset_0_0_100px_rgba(0,0,0,0.9),0_0_100px_rgba(235,0,40,0.1)]"
          style={{ transform: "translate3d(-50%, -50%, -200px)" }}
        />
        <div
          className="absolute bottom-0 left-1/2 w-[200vw] h-[40vh] bg-linear-to-t from-brand-500/20 to-transparent blur-[30px] origin-bottom"
          style={{ transform: "translateX(-50%) rotateX(75deg)" }}
        />
      </div>

      {sparks.map((spark) => (
        <div
          key={spark.id}
          className={`absolute rounded-full animate-float ${spark.s} ${spark.c} ${spark.b} ${spark.sh}`}
          style={{
            top: spark.t,
            left: spark.l,
            animationDelay: spark.d,
          }}
        />
      ))}
    </div>
  );
}
