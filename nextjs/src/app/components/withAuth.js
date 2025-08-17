import { useEffect } from "react";
import { useRouter } from "next/navigation";

import { useSession } from "next-auth/react";

const withAuth = (WrappedComponent) => {
  const AuthComponent = (props) => {
    const router = useRouter();

    useEffect(() => {
      // Check if username exists in localStorage
      const username =
        typeof window !== "undefined" ? localStorage.getItem("Username") : null;
      if (!username) {
        router.replace("/account/login");
      }
    }, [router]);

    // Optionally, you can show a loading state while checking
    const username =
      typeof window !== "undefined" ? localStorage.getItem("Username") : null;
    if (!username) {
      return null; // or a loading spinner
    }

    return <WrappedComponent {...props} />;
  };
  AuthComponent.displayName = `withAuth(${
    WrappedComponent.displayName || WrappedComponent.name || "Component"
  })`;

  return AuthComponent;
};

export default withAuth;
