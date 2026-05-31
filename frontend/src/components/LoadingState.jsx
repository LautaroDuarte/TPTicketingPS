import Spinner from "./Spinner";

/** Bloque centrado de carga, opcionalmente con un mensaje debajo. */
export default function LoadingState({ message }) {
  return (
    <div className="text-center py-5">
      <Spinner className="text-primary" />
      {message && <p className="mt-2">{message}</p>}
    </div>
  );
}