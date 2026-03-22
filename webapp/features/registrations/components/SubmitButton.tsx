'use client'

import { useFormStatus } from 'react-dom'
import { Button } from '@/components/ui/Button'

type SubmitButtonProps = {
  label?: string
  pendingLabel?: string
}

export function SubmitButton({
  label = 'Register',
  pendingLabel = 'Registering…',
}: SubmitButtonProps) {
  const { pending } = useFormStatus()

  return (
    <Button type="submit" disabled={pending} className="w-full">
      {pending ? (
        <>
          <span className="h-4 w-4 animate-spin rounded-full border-2 border-white border-t-transparent" />
          {pendingLabel}
        </>
      ) : (
        label
      )}
    </Button>
  )
}
