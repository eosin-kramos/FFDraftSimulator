import { useEffect, useState, ChangeEvent } from "react";

/* ─────────── 1.  Update the type  ─────────── */
interface PlayerRaw {
  player_id: string;            // use for unique key
  full_name: string;
  position: string;
  fantasy_positions: string[];  // <-- NEW
  status: string | null;        // <-- NEW ("Active", "Practice", etc.)
  team: string | null;
  age: number | null;
}

/* Offense-only positions allowed into state */
const OFFENSE = new Set(["QB", "RB", "WR", "TE", "K"]);
const POSITIONS = ["All", ...Array.from(OFFENSE)] as const;

export default function App() {
  const [players, setPlayers] = useState<Record<string, PlayerRaw>>({});
  const [loading, setLoading] = useState(true);
  const [posFilter, setPosFilter] =
    useState<(typeof POSITIONS)[number]>("All");

  /* ─────────── 2.  Fetch + pre-filter  ─────────── */
  useEffect(() => {
    fetch("/api/players")
      .then((r) => r.json())
      .then((data) =>
        Object.fromEntries(
          Object.entries(data).filter(
            ([, p]: [string, any]) =>
              p &&
              p.full_name &&
              p.status === "Active" &&
              Array.isArray(p.fantasy_positions) &&
              p.fantasy_positions.some((pos: string) => OFFENSE.has(pos.toUpperCase())) &&
              p.team && p.team !== "FA"
          )
        ) as Record<string, PlayerRaw>
      )
      .then(setPlayers)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const handleChange = (e: ChangeEvent<HTMLSelectElement>) =>
    setPosFilter(e.target.value as (typeof POSITIONS)[number]);

  if (loading) return <p style={{ padding: 16 }}>Loading…</p>;

  /* ─────────── 3.  Runtime filter & sort  ─────────── */
  const rows = Object.values(players)
    .filter(
      (p) =>
        (posFilter === "All" ||
          p.fantasy_positions?.some(
            (pos) => pos.toUpperCase() === posFilter
          )) &&
        p.team && p.team !== "FA"     
    )
    
    .sort((a, b) =>
      (a.full_name ?? "").localeCompare(b.full_name ?? "")
    );

  return (
    <main style={{ padding: 16, fontFamily: "sans-serif" }}>
      <h1 style={{ fontSize: 24, fontWeight: 700, marginBottom: 12 }}>
        Fantasy Draft Simulator (active NFL players)
      </h1>

      {/* position dropdown */}
      <label style={{ marginRight: 8 }}>
        Position:
        <select
          value={posFilter}
          onChange={handleChange}
          style={{ marginLeft: 4 }}
        >
          {POSITIONS.map((p) => (
            <option key={p} value={p}>
              {p}
            </option>
          ))}
        </select>
      </label>

      <table
        style={{ width: "100%", borderCollapse: "collapse", marginTop: 12 }}
      >
        <thead>
          <tr style={{ borderBottom: "2px solid #ccc" }}>
            <th style={{ textAlign: "left" }}>Name</th>
            <th>Pos</th>
            <th>Team</th>
            <th>Age</th>
          </tr>
        </thead>
        <tbody>
          {rows.map((p) => (
            <tr
              /* ── 4. better unique key ── */
              key={p.player_id}
              style={{ borderBottom: "1px solid #eee" }}
            >
              <td>{p.full_name}</td>
              <td align="center">{p.fantasy_positions[0]}</td>
              <td align="center">{p.team}</td>
              <td align="center">{p.age ?? "—"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </main>
  );
}
