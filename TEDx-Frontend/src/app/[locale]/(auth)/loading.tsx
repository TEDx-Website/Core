import { AuthSkeleton } from "@/features/auth/components/auth-skeleton";

export default function Loading() {
  return (
    <div className="w-full max-w-md mx-auto">
      <AuthSkeleton />
    </div>
  );
}
