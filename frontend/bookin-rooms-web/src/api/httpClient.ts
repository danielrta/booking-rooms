const AUTH_STORAGE_KEY = "booking_auth";

type HttpOptions = RequestInit & { auth?: boolean };

function getTokenFromStorage(): string | null {
  if (typeof window === "undefined") return null;

  const stored = localStorage.getItem(AUTH_STORAGE_KEY);
  if (!stored) return null;

  try {
    const parsed = JSON.parse(stored) as { token?: string };
    return parsed.token ?? null;
  } catch {
    return null;
  }
}

export async function http<T>(url: string, options: HttpOptions = {}): Promise<T> {
  const baseUrl = import.meta.env.VITE_API_BASE_URL;
  const token = getTokenFromStorage();

  const headers: HeadersInit = {
    "Content-Type": "application/json",
    ...(options.headers ?? {}),
  };

  if (options.auth !== false && token) {
    (headers as any).Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${baseUrl}${url}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `HTTP error ${response.status}`);
  }

  if (response.status === 204) {
    return {} as T;
  }

  return (await response.json()) as T;
}