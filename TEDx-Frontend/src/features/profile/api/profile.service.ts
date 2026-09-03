import { apiClient } from "@/lib/api";
import { UpdateProfileInput, ChangePasswordInput } from "../schema/profile.schema";
import {
  GetProfileResponse,
  UpdateProfileResponse,
  ProfilePictureUploadResponse,
  ChangePasswordResponse,
} from "../types/profile.types";

export const profileService = {
  getProfile: async (): Promise<GetProfileResponse> => {
    const response = await apiClient.get<GetProfileResponse>("/me");
    return response.data;
  },

  updateProfile: async (data: UpdateProfileInput): Promise<UpdateProfileResponse> => {
    const response = await apiClient.put<UpdateProfileResponse>("/me", data);
    return response.data;
  },

  changePassword: async (data: ChangePasswordInput): Promise<ChangePasswordResponse> => {
    const response = await apiClient.post<ChangePasswordResponse>("/me/change-password", data);
    return response.data;
  },

  uploadProfilePicture: async (file: File): Promise<ProfilePictureUploadResponse> => {
    const formData = new FormData();
    formData.append("file", file);

    const response = await apiClient.post<ProfilePictureUploadResponse>(
      "/me/profile-picture",
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );
    return response.data;
  },
};