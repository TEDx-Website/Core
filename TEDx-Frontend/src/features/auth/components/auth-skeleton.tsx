import { Card, CardHeader, CardContent, CardFooter } from "@/shared/ui/card";
import { Skeleton } from "@/shared/ui/skeleton";

export function AuthSkeleton() {
  return (
    <Card className="w-full border-0 shadow-none bg-transparent sm:bg-card sm:border sm:shadow-sm animate-in fade-in duration-500">
      <CardHeader className="px-0 sm:px-6 mb-2">
        <Skeleton className="h-8 w-40 mb-2 rounded-lg" />
        <Skeleton className="h-4 w-64 rounded-lg" />
      </CardHeader>

      <CardContent className="px-0 sm:px-6 space-y-5">
        <div className="space-y-2">
          <Skeleton className="h-4 w-20 rounded-md" />
          <Skeleton className="h-12 w-full rounded-xl" />
        </div>

        <div className="space-y-2">
          <div className="flex justify-between">
            <Skeleton className="h-4 w-24 rounded-md" />
            <Skeleton className="h-4 w-28 rounded-md" />
          </div>
          <Skeleton className="h-12 w-full rounded-xl" />
        </div>

        <Skeleton className="h-4 w-32 mt-2 rounded-md" />

        <Skeleton className="h-12 w-full mt-6 rounded-xl shadow-[0_0_15px_rgba(255,255,255,0.05)]" />

        <div className="relative my-6 text-center flex items-center justify-center">
          <Skeleton className="h-px w-full" />
          <Skeleton className="h-4 w-32 absolute bg-background sm:bg-card px-2" />
        </div>

        <div className="grid grid-cols-2 gap-3">
          <Skeleton className="h-12 w-full rounded-xl" />
          <Skeleton className="h-12 w-full rounded-xl" />
        </div>
      </CardContent>

      <CardFooter className="px-0 sm:px-6 justify-center pb-2">
        <Skeleton className="h-4 w-48 rounded-md" />
      </CardFooter>
    </Card>
  );
}
