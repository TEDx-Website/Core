import { useMutation } from "@tanstack/react-query";
import { submitContact } from "./contact.api";
import { ContactPayload } from "../types/contact.types";
import { BaseResponse, ApiError } from "@/shared/types/api";
export function useSubmitContact() {
  return useMutation<BaseResponse<null>, ApiError, ContactPayload>({
    mutationFn: submitContact,
  });
}