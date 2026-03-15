import { useEffect, useState } from "react";
import reactLogo from "./assets/react.svg";
import viteLogo from "/vite.svg";
import "./App.css";

function App() {
  const [fruits, setFruits] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch("http://localhost:8080/api/fruits")
      .then((res) => {
        if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
        return res.json();
      })
      .then((data) => {
        setFruits(data);
        setLoading(false);
      })
      .catch((err) => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  function deleteFruit(id) {
    fetch(`http://localhost:8080/api/fruits/${id}`, { method: "DELETE" })
      .then((res) => {
        if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
        setFruits((prevFruits) =>
          prevFruits.filter((fruit) => fruit.id !== id),
        );
      })
      .catch((err) => {
        setError(err.message);
      });
  }

  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <>
      <h1>Fruits</h1>

      <table border={1}>
        <thead>
          <th>Name</th>
          <th>Sweetness</th>
          <th>Image</th>
          <th>Actions</th>
        </thead>

        <tbody>
          {fruits.map((fruit) => (
            <tr key={fruit.id}>
              <td>{fruit.name}</td>
              <td>{fruit.sweetness}</td>
              <td>
                <img src={fruit.imageUrl} alt={fruit.name} width="100" />
              </td>
              <td>
                <button onClick={() => deleteFruit(fruit.id)}>
                  A Fucking Gift 🤓{" "}
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
}

export default App;
