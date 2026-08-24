import * as React from "react";
import { cn } from "@/lib/utils";

export interface GridContainerProps extends React.HTMLAttributes<HTMLDivElement> {
  as?: React.ElementType;
}

export const GridContainer = React.forwardRef<
  HTMLDivElement,
  GridContainerProps
>(({ className, as: Component = "div", children, ...props }, ref) => {
  return (
    <Component
      ref={ref}
      className={cn(
        "grid w-full mx-auto max-w-360",
        "grid-cols-4 px-[16px] gap-[16px]",
        "md:grid-cols-8 md:px-[32px] md:gap-[24px]",
        "lg:grid-cols-12 lg:px-[80px] lg:gap-[24px]",
        className,
      )}
      {...props}
    >
      {children}
    </Component>
  );
});
GridContainer.displayName = "GridContainer";
