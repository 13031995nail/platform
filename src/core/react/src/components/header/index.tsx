import { HStack } from '@tmobis/core/components/ui/hstack';
import { Pressable } from '@tmobis/core/components/ui/pressable';
import { Text } from '@tmobis/core/components/ui/text';
import { VStack } from '@tmobis/core/components/ui/vstack';
import { useRouter } from 'expo-router';
import { Image } from 'react-native';
import { useHeader } from './context';
import { HeaderLogoIcon } from './logo';
import { HeaderSystemControls } from './header-system-controls';

type HeaderProps = {
  title: string;
  number?: string | number;
  subtitle?: string;
};

const DynamicHeader = () => {
  const { headerData } = useHeader();

  return (
    <Header
      title={headerData.title}
      subtitle={headerData.subtitle}
    />
  );
};

const Header = ({ title, subtitle }: HeaderProps) => {
  const { logoSource } = useHeader();
  const router = useRouter();

  const handleLogoPress = () => {
    router.navigate('/desktop');
  };

  const logoImageComponent = logoSource
    ? <Image
      source={logoSource}
      className='h-8 w-[29px]'
      resizeMode='contain'
      accessibilityLabel='Т-Мобис'
    />
    : <HeaderLogoIcon />;

  const logoComponent =
    <Pressable
      accessibilityLabel='На главный экран'
      accessibilityRole='link'
      onPress={handleLogoPress}
      className='p-1'
    >
      {logoImageComponent}
    </Pressable>;

  const subtitleComponent = subtitle
    ? <Text
      size='xs'
      isTruncated
      className='mt-0.5 text-foreground'
    >
      {subtitle}
    </Text>
    : null;

  return (
    <HStack className='flex-1 items-center gap-2 max-w-full'>
      {logoComponent}
      <VStack className='shrink min-w-0 justify-center'>
        <Text
          size='lg'
          bold
          isTruncated
          className='text-foreground'
        >
          {title}
        </Text>
        {subtitleComponent}
      </VStack>
    </HStack>
  );
};

export {
  DynamicHeader,
  Header,
  HeaderLogoIcon,
  HeaderSystemControls,
};