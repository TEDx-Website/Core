import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { profileService } from "./profile.service";
import { UpdateProfileInput, ChangePasswordInput } from "../schema/profile.schema";

export const profileKeys = {
  all: ["profile"] as const,
  details: () => [...profileKeys.all, "details"] as const,
};

export const useProfile = () => {
  return useQuery({
    queryKey: profileKeys.details(),
    queryFn: () => profileService.getProfile(),
    staleTime: 5 * 60 * 1000,
  });
};

export const useUpdateProfile = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: UpdateProfileInput) => profileService.updateProfile(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: profileKeys.details() });
    },
  });
};

export const useChangePassword = () => {
  return useMutation({
    mutationFn: (data: ChangePasswordInput) => profileService.changePassword(data),
  });
};

export const useUploadProfilePicture = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (file: File) => profileService.uploadProfilePicture(file),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: profileKeys.details() });
    },
  });
};