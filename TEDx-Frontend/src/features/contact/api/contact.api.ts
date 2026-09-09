import { apiClient } from "@/lib/api";
import { BaseResponse } from "@/shared/types/api";
import { ContactPayload } from "../types/contact.types";

export const submitContact = async (data: ContactPayload): Promise<BaseResponse<null>> => {
  const response = await apiClient.post<BaseResponse<null>>("/contact", data);
  return response.data;
};