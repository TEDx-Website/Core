"use client";

import { forwardRef, useState } from "react";
import { Eye, EyeOff } from "lucide-react";

export interface PasswordInputProps extends React.InputHTMLAttributes<HTMLInputElement> {}

export const PasswordInput = forwardRef<HTMLInputElement, PasswordInputProps>(
  ({ className, ...props }, ref) => {
    const [showPassword, setShowPassword] = useState(false);

    return (
      <div className="relative">
        <input
          type={showPassword ? "text" : "password"}
          className={`w-full bg-neutral-100 dark:bg-[#121217] border border-neutral-200 dark:border-[#2B2B38] rounded-lg text-foreground px-4 py-3 text-sm transition-all focus:border-brand-500 focus:dark:bg-[#15151B] focus:ring-4 focus:ring-brand-500/20 outline-none placeholder:text-neutral-500 font-mono pr-10 ${className}`}
          ref={ref}
          {...props}
        />
        <button
          type="button"
          onClick={() => setShowPassword(!showPassword)}
          className="absolute right-3 top-1/2 -translate-y-1/2 text-neutral-400 hover:text-foreground p-1 transition-colors"
        >
          {showPassword ? (
            <Eye className="size-4" />
          ) : (
            <EyeOff className="size-4" />
          )}
        </button>
      </div>
    );
  },
);
PasswordInput.displayName = "PasswordInput";
