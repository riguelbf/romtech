export function Loader({ hasMore, loading, loader }:
  { hasMore: boolean, loading: boolean, loader: React.RefObject<HTMLDivElement | null> }) {
  return (
    <div
      ref={loader}
      className="flex justify-center items-center gap-2 py-6 rounded-md bg-zinc-700 border border-zinc-800 mt-4 text-sm text-white"
    >
      {hasMore ? (
        loading ? (
          <>
            <svg
              className="animate-spin h-5 w-5 text-muted-foreground"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
            >
              <circle
                className="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                strokeWidth="4"
              ></circle>
              <path
                className="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"
              ></path>
            </svg>
            <span className="animate-pulse bold">Loading more products...</span>
          </>
        ) : (
          <span>Scroll down to load more</span>
        )
      ) : (
        <span>No more products to load</span>
      )}
    </div>
  );
}