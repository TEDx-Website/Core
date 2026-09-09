import { getTranslations } from "next-intl/server";
import { ContactFormSection } from "@/features/contact/components/contact-form-section";
import { FAQSection } from "@/features/contact/components/faq-section";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  const t = await getTranslations({ locale, namespace: "contact" });

  return {
    title: `Contact Us | TEDxAlkawmia`,
  };
}

export default function ContactPage() {
  return (
    <main className="w-full overflow-hidden">
      <ContactFormSection />
      <FAQSection />
    </main>
  );
}
